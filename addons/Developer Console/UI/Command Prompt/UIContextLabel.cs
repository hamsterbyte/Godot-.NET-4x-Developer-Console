using System;
using Godot;
using System.Collections.Generic;
using System.Reflection;

namespace hamsterbyte.DeveloperConsole;

public partial class UIContextLabel : Label, ICanInitialize{

    private void ChangeContext((Node node, Dictionary<string, MethodInfo>) context){
        Text = context.node.PromptString();
    }

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UIContextLabel => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            CommandInterpreter.OnContextChanged += ChangeContext;
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }
}