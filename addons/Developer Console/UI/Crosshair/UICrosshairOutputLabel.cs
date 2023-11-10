using Godot;
using System;

namespace hamsterbyte.DeveloperConsole;

public partial class UICrosshairOutputLabel : RichTextLabel, ICanInitialize{

    #region INTERFACE

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UICrosshairOutputLabel => {success.OKFail()}");
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
    #endregion

    private void SetupCallbacks(){
        DC.OnPrint += UpdateOutputLabel;
    }

    private void UpdateOutputLabel(string message){
        CallThreadSafe("UpdateOutputLabelThreadSafe", message);
    }

    private void UpdateOutputLabelThreadSafe(string message){
        if (message.Trim().StartsWith("#!")){
            message = message.Replace("#!", $"[color={DCColorTheme.Regions[0].Color.ToHtml()}]");
            message = message.Replace("!#", $"[/color]");
        }

        if (message.Trim().StartsWith('$')){
            message = message.Insert(0, $"[color={DCColorTheme.Regions[1].Color.ToHtml()}]");
            message = message.Replace("::", "::[/color]");
        }

        Text = message;
    }
}
