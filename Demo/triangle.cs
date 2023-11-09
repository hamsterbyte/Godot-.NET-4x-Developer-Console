using Godot;
using System;
using hamsterbyte.DeveloperConsole;

public partial class triangle : Sprite2D{
    private bool _enableMove;
    private float _deltaAccumulated;
    private Vector2 _startPosition;

    [ConsoleCommand]
    private void ToggleMove() => _enableMove = !_enableMove;


    public override void _EnterTree(){
        _startPosition = GlobalPosition;
    }

    public override void _Process(double delta){
        if (!_enableMove) return;
        GlobalPosition = _startPosition + Vector2.Up * Mathf.Cos(_deltaAccumulated) * 128;
        _deltaAccumulated += (float)delta;
    }
}