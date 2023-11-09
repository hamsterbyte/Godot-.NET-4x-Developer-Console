using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using hamsterbyte.DeveloperConsole.Composition;

namespace hamsterbyte.DeveloperConsole;

public partial class UIContextTree : Tree, ICanInitialize{
    [Export] private Texture2D _visButtonTex;
    [Export] private Texture2D _invisButtonTex;
    [Export] private Camera2D _consoleCamera;
    [Export] private LineEdit _filter;

    private readonly Dictionary<Node, TreeItem> _associations = new();

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UIContextTree => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            SetupCallbacks();
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }

    private void SetupCallbacks(){
        ItemSelected += ChangeContext;
        ButtonClicked += ToggleNodeVisibility;
        DC.OnModeChanged += ValidateSceneComposition;
        SceneComposer.OnCompositionChanged += RecalculateNodeTree;
        Prefab.OnPrefabInstantiated += AddNodeToTree;
        Nodes.OnNodeDestroyed += RemoveNodeFromTree;
        _filter.TextChanged += FilterTree;
    }


    private void ValidateSceneComposition(int mode){
        if (!SceneComposer.RequiresDecomposition){
            RecalculateNodeTree();
        }

        switch (mode){
            case 0:
                SceneComposer.Compose();
                break;
            case 1:
                SceneComposer.Decompose();
                break;
            default:
                SceneComposer.Compose();
                break;
        }
    }

    private void RecalculateNodeTree(){
        ResetNodeTree();
        PopulateNodeTree();
    }

    private void ResetNodeTree(){
        Clear();
        _associations.Clear();
    }

    private void PopulateNodeTree(){
        foreach (Node n in DCCrosshair.Nodes){
            AddNodeToTree(n);
        }
    }

    private void FilterTree(string filter){
        foreach (KeyValuePair<Node, TreeItem> pair in _associations){
            pair.Value.Visible = false;
        }

        IEnumerable<KeyValuePair<Node, TreeItem>> validPairs =
            _associations.Where(pair => pair.Key.Name.ToString()!.ToLower().Contains(filter.ToLower()));

        foreach (KeyValuePair<Node, TreeItem> pair in validPairs){
            pair.Value.Visible = true;
            TreeItem parent = pair.Value.GetParent();
            while (parent is not null){
                parent.Visible = true;
                parent = parent.GetParent();
            }
        }
    }

    private void ToggleNodeVisibility(TreeItem item, long column, long id, long mousebuttonindex){
        item.SetButton(
            1,
            0,
            item.GetButton(1, 0) == _invisButtonTex
                ? _visButtonTex
                : _invisButtonTex
        );
        Node node = GetNode(item.GetPath());

        switch (node){
            case Control control:
                control.Visible = item.GetButton(1, 0) == _visButtonTex;
                break;
            case Node2D node2D:
                node2D.Visible = item.GetButton(1, 0) == _visButtonTex;
                break;
        }
    }

    private void AddNodeToTree(Node node){
        if (node is SubViewport or SubViewportContainer) return;
        Node parent = node.GetParentOrNull<Node>();
        TreeItem currentItem;
        if (parent is not null){
            _associations.TryGetValue(parent, out TreeItem parentItem);
            currentItem = CreateItem(parentItem);
        }
        else{
            currentItem = CreateItem();
        }

        _associations.Add(node, currentItem);
        currentItem.SetText(0, node.Name);
        currentItem.AddButton(1, _visButtonTex);
        currentItem.SetTooltipText(0, currentItem.GetPath());
    }

    private void RemoveNodeFromTree(Node n){
        if (!_associations.TryGetValue(n, out TreeItem t)) return;
        t.Free();
        _associations.Remove(n);
    }

    private void ChangeContext(){
        TreeItem item = GetSelected();

        if (GetNodeOrNull<Node>(item.GetPath()) is null){
            item.Free();
            DC.Print("This node no longer exists.");
            return;
        }

        DC.ChangeContext(item.GetPath());
    }
}