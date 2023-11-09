using System;
using System.Collections.Generic;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static class Level{
    public static DCDelegates.onCallback OnSceneLoaded;
    private static List<ResourceInformation> _sceneInfos = new();

    // ReSharper disable once MemberCanBePrivate.Global
    public static List<ResourceInformation> SceneInfos{
        get{
            if (_sceneInfos.Count == 0){
                InitializeScenes();
            }

            return _sceneInfos;
        }
    }

    private static void InitializeScenes(){
        DC.Print("Initializing Levels List...");
        Resources.GetResourceInformation(ResourcePaths.Levels, ref _sceneInfos);
    }

    [ConsoleCommand(Prefix = "Level", Description = "Print a table of all scenes in the Levels folder")]
    private static void GetAll(){
        for (int i = 0; i < SceneInfos.Count; i++){
            DC.Print($"ID:: {i.ToString().SetWidth(20)}{SceneInfos[i]}");
        }
    }
    
    [ConsoleCommand(Prefix = "Level",
        Description =
            "Load a level by index, overwriting the current context. Level.GetAll() to print a table of all levels.")]
    private static void Load(int index){
        if (DC.Mode == 1) throw new DCException("Cannot load levels from Crosshair Mode, switch to terminal.");
        try{
            PackedScene p = ResourceLoader.Load<PackedScene>(SceneInfos[index].Path);
            foreach (Node n in DC.CurrentNode.GetChildren()){
                n.QueueFree();
            }

            DC.CurrentNode.ReplaceBy(p.Instantiate());
            DC.CurrentNode.QueueFree();
            OnSceneLoaded?.InvokeAwaited();
        }
        catch (Exception e){
            throw new DCException($"Failed to load level {e.Message}");
        }
    }

    [ConsoleCommand(Prefix = "Level",
        Description = "Load a level by index into the current context. Level.GetAll() to print a table of all levels.")]
    private static void LoadAdditive(int index){
        if (DC.Mode == 1) throw new DCException("Cannot load levels from Crosshair Mode, switch to terminal.");
        try{
            PackedScene p = ResourceLoader.Load<PackedScene>(SceneInfos[index].Path);
            DC.CurrentNode.AddChild(p.Instantiate());
            OnSceneLoaded?.InvokeAwaited();
        }
        catch (Exception e){
            throw new DCException($"Failed to load level {e.Message}");
        }
    }
}