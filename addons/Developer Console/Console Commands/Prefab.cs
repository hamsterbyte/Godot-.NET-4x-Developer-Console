using System;
using System.Collections.Generic;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static class Prefab{
    public static DCDelegates.onPrefabInstantiated OnPrefabInstantiated;
    private static List<ResourceInformation> _prefabInfos = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public static List<ResourceInformation> PrefabInfos{
        get{
            if (_prefabInfos.Count == 0){
                InitializePrefabInfos();
            }

            return _prefabInfos;
        }
    }

    private static void InitializePrefabInfos(){
        DC.Print("Initializing Prefabs List...");
        Resources.GetResourceInformation(ResourcePaths.Prefabs, ref _prefabInfos);
    }

    [ConsoleCommand(Prefix = "Prefab", Description = "Print a list of all prefabs and their pertinent information")]
    private static void GetAll(){
        for (int i = 0; i < PrefabInfos.Count; i++){
            DC.Print($"ID:: {i.ToString().SetWidth(20)}{PrefabInfos[i]}");
        }
    }

    [ConsoleCommand(Prefix = "Prefab",
        Description = "Print a list of all prefabs with a matching name. Not case sensitive")]
    private static void Find(string name){
        for (int i = 0; i < PrefabInfos.Count; i++){
            if (PrefabInfos[i].Name.ToLower().Contains(name.ToLower())){
                DC.Print($"ID:: {i.ToString().SetWidth(20)}{PrefabInfos[i]}");
            }
        }
    }

    [ConsoleCommand(Prefix = "Prefab",
        Description = "Instantiate a prefab as a child of the scene root positioned at the mouse")]
    private static void Instantiate(int index){
        try{
            PackedScene p = ResourceLoader.Load<PackedScene>(PrefabInfos[index].Path);
            Node n = p.Instantiate<Node>();
            DC.CurrentNode.AddChild(n);
            switch (n){
                case Node2D node2D:
                    node2D.GlobalPosition = node2D.GetGlobalMousePosition();
                    break;
                case Control control:
                    control.GlobalPosition = control.GetGlobalMousePosition();
                    break;
            }

            DCCrosshair.Nodes.Add(n);
            OnPrefabInstantiated?.InvokeAwaited(n);
        }
        catch (Exception e){
            throw new DCException($"Failed to instantiate prefab {e.Message}");
        }
    }
}