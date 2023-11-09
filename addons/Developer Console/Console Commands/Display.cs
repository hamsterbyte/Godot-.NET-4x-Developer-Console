using System;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public partial class Display{
    [ConsoleCommand(Prefix = "Display", Description = "Set Vsync mode. Use GetVsyncModes() for a list of valid options. Method expects int")]
    private static void SetVsyncMode(int mode){
        DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)mode);
    }

    [ConsoleCommand(Prefix = "Display", Description = "Print the current Vsync mode to the console")]
    private static void GetCurrentVsyncMode(){
        DC.Print((int)DisplayServer.WindowGetVsyncMode());
    }

    [ConsoleCommand(Prefix = "Display", Description = "Print a list of all valid vsync mode indices and names")]
    private static void GetVsyncModes(){
        string[] modes = Enum.GetNames(typeof(DisplayServer.VSyncMode));
        for (int i = 0; i < modes.Length; i++){
            DC.Print($"{i} - {modes[i]}");
        }
    }

    [ConsoleCommand(Prefix = "Display", Description = "Print the current mouse position")]
    private static void GetMousePosition(){
        DC.Print(DC.Root.GetViewport().GetMousePosition());
    }
}