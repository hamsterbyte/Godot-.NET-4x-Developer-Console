using System.Collections.Generic;
using System.Linq;
using Godot;

namespace hamsterbyte.DeveloperConsole.Composition;

public static class SceneComposer{
    public static bool RequiresDecomposition => DCCrosshair.Nodes.Any(n => n is SubViewport or SubViewportContainer);
    private static bool IsDecomposed;
    private static readonly Stack<ICompositionCommand> _decomposeHistory = new();
    private static readonly Stack<ICompositionCommand> _composeHistory = new();
    public static DCDelegates.onCallback OnCompositionChanged;

    public static void Compose(){
        if (!IsDecomposed) return;
        DC.PrintComment($"Composing Scene");
        while (_composeHistory.Count > 0){
            ICompositionCommand current = _composeHistory.Pop();
            current.Undo();
        }

        IsDecomposed = false;
        DC.ChangeContext("/root");
        OnCompositionChanged?.InvokeAwaited();
    }

    public static void Decompose(){
        if (!RequiresDecomposition || IsDecomposed) return;
        DC.PrintComment("Decomposing Scene");
        foreach (Node n in DCCrosshair.Nodes){
            switch (n){
                case SubViewportContainer subViewportContainer:
                    _decomposeHistory.Push(new SetInvisibleCommand(subViewportContainer));
                    break;
                case SubViewport subViewport:
                    foreach (Node child in subViewport.GetChildren()){
                        _decomposeHistory.Push(new ReparentCommand(child));
                    }
                    break;
            }
        }

        while (_decomposeHistory.Count > 0){
            ICompositionCommand current = _decomposeHistory.Pop();
            _composeHistory.Push(current);
            current.Execute();
        }

        IsDecomposed = true;
        DC.ChangeContext("/root");
        OnCompositionChanged?.InvokeAwaited();
    }
}