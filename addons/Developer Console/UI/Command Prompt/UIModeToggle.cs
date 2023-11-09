using System;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public partial class UIModeToggle : TextureButton, ICanInitialize{
    [Export] private Control _crosshairMode;
    [Export] private Control _terminalMode;

    public override void _EnterTree(){
        SetupCallbacks();
    }

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UIModeToggle => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }

    private void SetupCallbacks(){
        Toggled += ToggleModePanels;
    }

    private void ToggleModePanels(bool toggle){
        _crosshairMode.Visible = !_crosshairMode.Visible;
        _terminalMode.Visible = !_terminalMode.Visible;
        DC.ChangeMode(_terminalMode.Visible ? 0 : 1);
    }
}