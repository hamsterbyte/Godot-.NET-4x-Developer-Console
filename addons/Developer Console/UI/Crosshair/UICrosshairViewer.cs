using Godot;
using System;

namespace hamsterbyte.DeveloperConsole;

public partial class UICrosshairViewer : PanelContainer, ICanInitialize{
    #region VARIABLES

    [ExportGroup("Settings")] [Export] private Color _crosshairColor = Colors.Chartreuse;
    [ExportGroup("Controls")] [Export] private Camera2D _consoleCamera;

    public bool Enabled{ get; private set; }
    private bool _isCrosshairMode;
    private Vector2 hStart = Vector2.Zero;
    private Vector2 hEnd = Vector2.Zero;
    private Vector2 vStart = Vector2.Zero;
    private Vector2 vEnd = Vector2.Zero;
    private Vector2 _currentMousePosition;
    private const float _zoomStep = .1f;
    private Vector2 _minZoom = new(.1f, .1f);
    private Vector2 _maxZoom = new(10, 10);
    private bool _dragCamera;
    private bool _dragNode;

    #endregion

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UICrosshairViewer => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            MouseEntered += EnableCrosshair;
            MouseExited += DisableCrosshair;
            DC.OnModeChanged += ModeChanged;
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }


    public override void _Process(double delta){
        if (!DC.Enabled || !_isCrosshairMode) return;
        UpdateCrosshair();
    }

    public override void _Input(InputEvent @event){
        if (!Enabled) return;
        if (@event is not InputEventMouse m) return;
        TryDragCamera(m);
        TryDragNode(m);
        TryZoomCamera(m);
    }

    private void ModeChanged(int mode){
        _isCrosshairMode = mode == 1;
    }

    private void EnableCrosshair(){
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        hEnd.X = GetRect().End.X;
        vEnd.Y = GetRect().End.Y;
        Enabled = true;
    }

    private void DisableCrosshair(){
        Input.MouseMode = Input.MouseModeEnum.Visible;
        Enabled = false;
        _dragCamera = false;
        _dragNode = false;
    }

    public override void _Draw(){
        if (!Enabled) return;
        DrawDashedLine(hStart, hEnd, _crosshairColor, 2);
        DrawDashedLine(vStart, vEnd, _crosshairColor, 2);
    }

    private void UpdateCrosshair(){
        QueueRedraw();
        if (!Enabled) return;
        _currentMousePosition = GetLocalMousePosition();
        hStart.Y = _currentMousePosition.Y;
        hEnd.Y = _currentMousePosition.Y;
        vStart.X = _currentMousePosition.X;
        vEnd.X = _currentMousePosition.X;
    }

    private void TryDragCamera(InputEventMouse m){
        switch (m){
            case InputEventMouseButton b:
                _dragCamera = b.Pressed && Enabled && DC.Enabled && b.ButtonIndex == MouseButton.Middle;
                break;
            case InputEventMouseMotion v:
                if (_dragCamera) _consoleCamera.GlobalPosition -= v.Relative * (Vector2.One / _consoleCamera.Zoom);
                break;
        }
    }

    private void TryDragNode(InputEventMouse m){
        switch (m){
            case InputEventMouseButton b:
                _dragNode = b.Pressed && DC.Enabled && b.ButtonIndex == MouseButton.Left &&
                            b.ShiftPressed && Enabled;
                break;
            case InputEventMouseMotion v:
                if (_dragNode){
                    switch (DC.CurrentNode){
                        case Control control:
                            control.GlobalPosition += v.Relative * (Vector2.One / _consoleCamera.Zoom);
                            break;
                        case Node2D node2D:
                            node2D.GlobalPosition += v.Relative * (Vector2.One / _consoleCamera.Zoom);
                            break;
                    }
                }

                break;
        }
    }


    private void ZoomCamera(Vector2 deltaZoom){
        Vector2 newZoom = new(
            Mathf.Clamp(_consoleCamera.Zoom.X + deltaZoom.X, _minZoom.X, _maxZoom.X),
            Mathf.Clamp(_consoleCamera.Zoom.Y + deltaZoom.Y, _minZoom.Y, _maxZoom.Y)
        );
        _consoleCamera.Zoom = newZoom;
    }

    private void TryZoomCamera(InputEventMouse m){
        switch (m){
            case InputEventMouseButton b:
                if (_consoleCamera.Enabled){
                    if (!b.Pressed) return;
                    switch (b.ButtonIndex){
                        case MouseButton.WheelUp:
                            ZoomCamera(Vector2.One * _zoomStep);
                            break;
                        case MouseButton.WheelDown:
                            ZoomCamera(-Vector2.One * _zoomStep);
                            break;
                    }
                }

                break;
        }
    }
}