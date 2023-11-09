using Godot;
using System;
using hamsterbyte.DeveloperConsole;

public partial class UIDescriptionLabel : RichTextLabel{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta){
        if (!Visible || !DC.Enabled) return;
        GlobalPosition = GetGlobalMousePosition() - PivotOffset;
    }
}