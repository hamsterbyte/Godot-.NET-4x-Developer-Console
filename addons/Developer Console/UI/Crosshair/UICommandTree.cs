using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using hamsterbyte.DeveloperConsole;

public partial class UICommandTree : Tree, ICanInitialize{
    [Export] private UIAutocompleteController _autocompleteController;

    private TreeItem _instanceCommandItem;
    private TreeItem _staticCommandItem;
    private Dictionary<string, MethodInfo> _contextCommands;

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UICommandTree => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            SetupTree();
            SetupCallbacks();
            SetupCommands();
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }

    private void SetupTree(){
        Clear();
        TreeItem root = CreateItem();
        root.SetText(0, "root");
    }

    private void SetupCallbacks(){
        CommandInterpreter.OnGetStaticCommands += PopulateStaticCommands;
        CommandInterpreter.OnGetInstanceCommands += PopulateInstanceCommands;
        ItemSelected += SendCommandToPrompt;
    }

    private void SetupCommands(){
        PopulateStaticCommands(CommandInterpreter.StaticCommands);
        PopulateInstanceCommands(CommandInterpreter.InstanceCommands);
    }


    private void SendCommandToPrompt(){
        if (GetSelected().GetMetadata(0).AsBool()){
            _autocompleteController.SetCommandPromptFromCommandTree(GetSelected().GetText(0));
        }
    }

    private void PopulateInstanceCommands(Dictionary<string, MethodInfo> commands){
        _contextCommands = commands;
        _instanceCommandItem?.Free();
        _instanceCommandItem = null;
        if (_contextCommands is null || _contextCommands.Count == 0) return;
        _instanceCommandItem = CreateItem(null, 0);
        _instanceCommandItem.SetText(0, "Instance");
        _instanceCommandItem.SetMetadata(0, false);
        foreach (KeyValuePair<string, MethodInfo> v in _contextCommands){
            TreeItem currentItem = CreateItem(_instanceCommandItem);
            currentItem.SetText(0, v.Key);
            currentItem.SetMetadata(0, true);
        }
    }

    private void PopulateStaticCommands(Dictionary<string, MethodInfo> commands){
        Dictionary<string, TreeItem> commandDictionary = new();
        _staticCommandItem = CreateItem();
        _staticCommandItem.SetText(0, "Static");
        _staticCommandItem.SetMetadata(0, false);

        foreach (KeyValuePair<string, MethodInfo> v in commands){
            string[] parts = v.Key.Split('.');
            if (parts.Length > 1){
                if (!commandDictionary.ContainsKey(parts[0])){
                    TreeItem parentItem = CreateItem(_staticCommandItem);
                    parentItem.SetText(0, parts[0]);
                    parentItem.SetMetadata(0, false);
                    commandDictionary.Add(parts[0], parentItem);
                }
            }

            TreeItem currentItem = CreateItem(commandDictionary.TryGetValue(parts[0], out TreeItem pItem)
                ? commandDictionary[parts[0]]
                : _staticCommandItem);
            currentItem.SetText(0, v.Key);
            currentItem.SetTooltipText(0, v.Value.GetCustomAttribute<ConsoleCommandAttribute>()?.Description);
            currentItem.SetMetadata(0, true);
        }

        foreach (KeyValuePair<string, TreeItem> c in commandDictionary){
            c.Value.Collapsed = true;
        }
    }
}