using System;
using System.Collections.Generic;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public partial class UITerminalOutput : CodeEdit, ICanInitialize{
    [Export] private UICommandInput _commandInput;
    private CodeHighlighter _highlighter;

    public override void _EnterTree(){
        DC.OnPrint += PrintToOutput;
    }

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UITerminalOutput => {success.OKFail()}");
    }

    public bool TryInitialize(){
        try{
            SetupCallbacks();
            ApplyColorTheme();
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }

    private void SetupCallbacks(){
        DC.OnClear += ClearOutput;
        GutterClicked += CopyToInput;
        DC.OnCommandCompleted += GoToBottom;
    }

    #region CALLBACKS

    private void PrintToOutput(string message){
        CallThreadSafe("PrintThreadSafe", message);
    }

    private void PrintThreadSafe(string message){
        SetLine(GetLineCount() - 1, $"{message}\n");
        CallDeferred("ScrollToEnd");
    }

    private void ScrollToEnd(){
        ScrollVertical = GetLineCount();
    }

    private void ClearOutput(){
        ClearUndoHistory();
        SelectAll();
        DeleteSelection();
    }

    private void CopyToInput(long line, long gutter){
        if (gutter != 1) return;
        Deselect();
        _commandInput.CopyLineToInput(line);
    }

    private void GoToBottom(){
        SetCaretLine(GetLineCount() - 1);
    }

    #endregion

    #region APPLY COLOR THEME

    private void ApplyColorTheme(){
        _highlighter = (CodeHighlighter)SyntaxHighlighter;
        SetRegionColors();
        SetKeywordColors();
        SetOutputColors();
    }

    private void SetRegionColors(){
        foreach (RegionColor rC in DCColorTheme.Regions){
            _highlighter?.AddColorRegion(rC.Start, rC.End, rC.Color);
        }
    }

    private void SetKeywordColors(){
        if (_highlighter is null) return;
        foreach (KeyValuePair<string, string> typeReplacements in DCExtensions.TypeReplacementStrings){
            if (_highlighter.KeywordColors.ContainsKey(typeReplacements.Value)){
                _highlighter.KeywordColors[typeReplacements.Value] = DCColorTheme.Output["Member Variable"];
            }
            else{
                _highlighter.AddKeywordColor(typeReplacements.Value, DCColorTheme.Output["Member Variable"]);
            }
        }
    }

    private void SetOutputColors(){
        foreach (KeyValuePair<string, Color> color in DCColorTheme.Output){
            switch (color.Key){
                case "Method":
                    _highlighter.FunctionColor = color.Value;
                    break;
                case "Number":
                    _highlighter.NumberColor = color.Value;
                    break;
                case "Symbol":
                    _highlighter.SymbolColor = color.Value;
                    break;
            }
        }
    }

    #endregion
}
