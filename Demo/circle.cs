using Godot;
using System;
using hamsterbyte.DeveloperConsole;

public partial class circle : RigidBody2D{
    [ConsoleCommand]
    private void ToggleFreeze(){
        Freeze = !Freeze;
    }

    [ConsoleCommand]
    private void AddForce(int x, int y){
        SetAxisVelocity(new Vector2(x, y));
    }
}