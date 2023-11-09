using Godot;
using System;
using hamsterbyte.DeveloperConsole;

public partial class UIMousePositionLabel : Label{
    public override void _Process(double delta){
        if (DC.Enabled){
            Text = ((Vector2I)GetGlobalMousePosition()).ToString();
        }
    }
}