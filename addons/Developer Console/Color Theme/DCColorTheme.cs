using System.Collections.Generic;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static class DCColorTheme{
    public static readonly Dictionary<string, Color> Output = new(){
        { "Number", Colors.White },
        { "Symbol", Colors.White },
        { "Method", Colors.DeepSkyBlue },
        { "Member Variable", Colors.ForestGreen }
    };

    public static readonly RegionColor[] Regions ={
        //Error color
        new(){ Color = Colors.DeepPink, Start = "#!", End = "!#" },
        //Context Color
        new(){ Color = Colors.Goldenrod, Start = "$", End = "::" },
        //Comment Color
        new(){ Color = Colors.PaleGreen, Start = "/*", End = "*/" }
    };

    public static readonly Dictionary<string, string> Suggestions = new(){
        { "Method", Colors.DeepSkyBlue.ToHtml() },
        { "Type", Colors.ForestGreen.ToHtml() }
    };
}

public struct RegionColor{
    public string Start;
    public string End;
    public Color Color;
}