using Godot;
using System;
using System.Collections.Generic;
using hamsterbyte.DeveloperConsole;

public partial class ConsoleCamera : Camera2D, ICanInitialize{
    private readonly List<CameraInformation> _cameraInformations = new();

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"ConsoleCamera => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            SetupCallbacks();
            GetAllCameras();
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }

    private void SetupCallbacks(){
        DC.OnModeChanged += SwapControl;
        Level.OnSceneLoaded += GetAllCameras;
    }

    private void SwapControl(int mode){
        if (mode == 1){
            //Crosshair
            TakeControl();
        }
        else{
            ReturnControl();
        }
    }

    private void GetAllCameras(){
        _cameraInformations.Clear();
        foreach (Node n in DCCrosshair.Nodes){
            if (n is Camera2D camera2D){
                _cameraInformations.Add(new CameraInformation(){
                    Camera = camera2D,
                    Enabled = camera2D.Enabled,
                    IsCurrent = camera2D.IsCurrent()
                });
            }
        }
    }


    private void TakeControl(){
        Enabled = true;
        MakeCurrent();
        foreach (CameraInformation info in _cameraInformations){
            if (!CameraIsValid(info)) continue;
            info.Camera.Enabled = false;
        }
    }

    private void ReturnControl(){
        Enabled = false;
        foreach (CameraInformation info in _cameraInformations){
            if (!CameraIsValid(info)) continue;
            info.Camera.Enabled = info.Enabled;
            if (info.IsCurrent) info.Camera.MakeCurrent();
        }
    }

    private bool CameraIsValid(CameraInformation info){
        if (!IsInstanceValid(info.Camera)){
            _cameraInformations.Remove(info);
            return false;
        }

        return true;
    }

    private struct CameraInformation{
        public Camera2D Camera;
        public bool Enabled;
        public bool IsCurrent;
    }
}