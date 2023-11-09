using System;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public partial class CommandInterpreter{
    public static DCDelegates.onContextChanged OnContextChanged;

    public static void ChangeContext(string path){
        Node selectedNode;
        if (path.StartsWith("/.")){
            ContextLevelUp(path.TrimStart('/').AsSpan(), out selectedNode);
        }
        else{
            ContextFromPath(path, out selectedNode);
        }

        if (selectedNode is null) throw new DCException($"'{path}' does not exist.");

        UpdateContextCommands(selectedNode);
        _currentNode = selectedNode;
        OnContextChanged?.Invoke(Context);
    }

    private static void ContextLevelUp(ReadOnlySpan<char> trailingSpan, out Node selectedNode){
        selectedNode = _currentNode;
        for (int i = 0; i < trailingSpan.Length; i++){
            if (selectedNode is null || trailingSpan[i] != '.') break;
            Node parent = selectedNode.GetParentOrNull<Node>();
            if (parent is not null){
                selectedNode = parent;
            }
            else{
                break;
            }
        }
    }

    private static void ContextFromPath(string path, out Node selectedNode){
        selectedNode = DC.Instance.GetNodeOrNull(
            path.StartsWith("/root")
                ? path //path is absolute
                : $"{DC.CurrentNode.GetPath()}{path}" //path is relative
        );
    }
}