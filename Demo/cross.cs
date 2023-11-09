using Godot;
using System;
using hamsterbyte.DeveloperConsole;

public partial class cross : Sprite2D{
    private bool _scaleEnabled;
    private float _deltaAccumulated;

    [ConsoleCommand]
    private void ToggleScaleAnimation(){
        _scaleEnabled = !_scaleEnabled;
    }

    public override void _Process(double delta){
        if (!_scaleEnabled) return;
        Scale = Vector2.One + Vector2.One * Mathf.Abs(Mathf.Sin(_deltaAccumulated));
        _deltaAccumulated += (float)delta;
    }
}