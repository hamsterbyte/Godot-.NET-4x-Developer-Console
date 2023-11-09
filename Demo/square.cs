using Godot;
using System;
using hamsterbyte.DeveloperConsole;

public partial class square : Sprite2D{
    private bool _enableRotate;

    [ConsoleCommand]
    public void ToggleRotate() => _enableRotate = !_enableRotate;

    [ConsoleCommand]
    private void FailOnPurpose(){
        int[] ar = new int[24];
        Array.Fill(ar, 0);
        for (int i = -1; i < ar.Length; i++){
            DC.Print(ar[i]);
        }
    }

    public override void _Process(double delta){
        if (!_enableRotate) return;
        Rotation += (float)delta;
    }
}