using System;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public partial class UICommandInput : LineEdit, ICanInitialize{
    [ExportGroup("Controls")] [Export] private CodeEdit _terminalMode;
    [Export] private Label _contextLabel;
    [Export] private UIAutocompleteController _autocompleteController;


    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UICommandInput => {success.OKFail()}");
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
        TextSubmitted += SubmitCommand;
    }

    private void SubmitCommand(string commandString){
        if (commandString == "") return;
        if (TryCopyLineShortcut(commandString)) return;
        Submit(commandString);
        Clear();
    }

    private void Submit(string commandString){
        _autocompleteController.CompleteAndSubmit(commandString);
    }

    private bool TryCopyLineShortcut(string commandString){
        try{
            if (commandString.StartsWith('#')){
                long lineNumber = long.Parse(commandString.TrimStart('#'));
                CopyLineToInput(lineNumber - 1);
                return true;
            }
        }
        catch (Exception e){
            Clear();
            DC.PrintError($"Cannot copy line ({e.Message})");
        }

        return false;
    }

    public void CopyLineToInput(long line){
        _terminalMode.Deselect();
        Text = _terminalMode.GetLine((int)line).TrimStart(DC.CurrentNode.PromptString().ToCharArray()).Trim();
        CaretColumn = Text.Length;
        GrabFocus();
    }
}