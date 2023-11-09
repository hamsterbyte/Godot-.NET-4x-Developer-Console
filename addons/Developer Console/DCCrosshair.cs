using System.Collections.Generic;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static partial class DCCrosshair{
    public static List<Node> Nodes{ get; } = new();
    public static DCDelegates.onCallback OnNodesUpdated;

    #region INITALIZE

    public static void Initialize(){
        bool success = TryInitialize();
        DC.Print($"Crosshair Mode => {success.OKFail()}");
        DC.Instance.InitializeUI();
    }

    private static bool TryInitialize(){
        try{
            DC.SetSuppressionLevel(DC.PrintSuppression.All);
            GetAllNodes();
            Level.OnSceneLoaded += GetAllNodes;
            DC.SetSuppressionLevel(DC.PrintSuppression.GD);
            return true;
        }
        catch{
            return false;
        }
    }

    #endregion

    private static void AddNode(Node node){
        if (!Nodes.Contains(node)){
            Nodes.Add(node);
        }
    }

    public static void GetAllNodes(){
        Nodes.Clear();
        Nodes.Add(DC.Root);
        DC.ChangeContext(DC.Root.GetPath());
        if (DC.Root.GetChildCount() != 0){
            GetChildNodes(DC.Root);
        }
        else{
            throw new DCException("Tree contains no nodes");
        }

        OnNodesUpdated?.InvokeAwaited();
    }

    private static void GetChildNodes(Node targetNode){
        if (targetNode.GetChildCount() <= 0) return;
        foreach (Node node in targetNode.GetChildren()){
            if (node.GetPath().ToString()!.Contains("DeveloperConsole")) continue;
            AddNode(node);
            GetChildNodes(node);
        }
    }
}