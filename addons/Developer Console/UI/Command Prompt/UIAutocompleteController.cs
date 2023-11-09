using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace hamsterbyte.DeveloperConsole;

public partial class UIAutocompleteController : Control, ICanInitialize{
    [ExportGroup("Controls")] [Export] private Label _contextLabel;
    [Export] private Label _autoCompleteLabel;
    [Export] private RichTextLabel _autoCompleteSuggestions;
    [Export] private RichTextLabel _autoCompleteDescription;
    [Export] private LineEdit _commandInput;

    private string[] _suggestionStrings = Array.Empty<string>();
    private int _currentShift;
    private readonly Dictionary<string, string> _commands = new();
    private readonly Dictionary<string, string> _descriptions = new();

    #region SETUP

    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"UIAutocompleteController => {success.OKFail()}");
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
        DC.OnDisable += Disable;
        DC.OnEnable += Enable;
        _autoCompleteSuggestions.MetaHoverStarted += MetaHoverStart;
        _autoCompleteSuggestions.MetaHoverEnded += MetaHoverEnd;
        _autoCompleteSuggestions.MetaClicked += OnMetaClicked;
        CommandInterpreter.OnContextChanged += ChangeContext;
    }


    private void Enable(){
        _commandInput.GrabFocus();
        _commandInput.Clear();
    }

    private void Disable(){
        _autoCompleteDescription.Visible = false;
        _autoCompleteDescription.Text = string.Empty;
        _autoCompleteSuggestions.Visible = false;
        _autoCompleteSuggestions.Text = string.Empty;
        _autoCompleteLabel.Text = string.Empty;
        _commandInput.ReleaseFocus();
    }

    private void ChangeContext((Node, Dictionary<string, MethodInfo> commands) context){
        SetConsoleCommands(context.commands);
    }

    #endregion

    #region INPUT

    public override void _Input(InputEvent @event){
        if (@event is not InputEventKey k) return;
        if (!DC.Enabled) return;
        if (k.Pressed){
            HandleVisiblePressKeystrokes(k);
        }
        else{
            UpdateAutocomplete();
            TryCorrectCase(k);
        }
    }

    private void HandleVisiblePressKeystrokes(InputEventKey k){
        if (!_commandInput.HasFocus()) return;
        switch (k.Keycode){
            case Key.Up:
                KeyUpPressedVisible();
                break;
            case Key.Down:
                KeyDownPressedVisible();
                break;
            case Key.Right:
                KeyRightPressedVisible();
                break;
        }
    }

    private void KeyUpPressedVisible(){
        if (!_autoCompleteSuggestions.Visible){
            PopHistory();
        }
        else{
            ShiftSuggestions(1);
        }
    }

    private void KeyDownPressedVisible(){
        if (!_autoCompleteSuggestions.Visible) return;
        ShiftSuggestions(-1);
    }

    private void KeyRightPressedVisible(){
        if (_autoCompleteLabel.Text == string.Empty || _commandInput.CaretColumn != _commandInput.Text.Length) return;
        _commandInput.Text = _autoCompleteLabel.Text;
        _commandInput.CaretColumn = _commandInput.Text.Length - 2;
    }

    #endregion

    public override void _Process(double delta){
        if (!_autoCompleteSuggestions.Visible) return;
        CallDeferred(nameof(PositionSuggestions));
    }

    private void SetConsoleCommands(Dictionary<string, MethodInfo> commands){
        _commands.Clear();
        _descriptions.Clear();
        foreach (KeyValuePair<string, MethodInfo> command in commands){
            _commands.Add($"{command.Key}()", command.Value.ParamsAsString());
            _descriptions.Add($"{command.Key}()",
                command.Value.GetCustomAttribute<ConsoleCommandAttribute>()?.Description);
        }
    }

    private void PositionSuggestions(){
        Vector2 newPos = new(){
            X = _contextLabel.Size.X + 10,
            Y = GetViewport().GetVisibleRect().Size.Y - _autoCompleteSuggestions.Size.Y - 42
        };
        _autoCompleteSuggestions.GlobalPosition = newPos;
    }


    private void MetaHoverStart(Variant meta){
        _autoCompleteDescription.ResetSize();
        _autoCompleteDescription.Visible = true;
        _autoCompleteDescription.Text = meta.AsString().Split('|')[1];
    }

    private void MetaHoverEnd(Variant meta){
        _autoCompleteDescription.Visible = false;
        _autoCompleteDescription.Text = string.Empty;
    }

    private void OnMetaClicked(Variant meta){
        _commandInput.Text = _autoCompleteDescription.Text = meta.AsString().Split('|')[0];
        _commandInput.CaretColumn = _commandInput.Text.Length - 1;
        UpdateAutocomplete();
    }

    public void CompleteAndSubmit(string commandString){
        if (_autoCompleteLabel.Text.StartsWith(commandString)){
            commandString = _autoCompleteLabel.Text;
        }

        DC.Submit(commandString);
    }

    private void UpdateAutoCompleteLabel(){
        _autoCompleteLabel.Text = _suggestionStrings.Length == 0
            ? string.Empty
            : _suggestionStrings[_currentShift.Wrap(_suggestionStrings.Length)];
        if (_suggestionStrings.Length == 0) return;
        if (_commandInput.Text.Length > _suggestionStrings[_currentShift.Wrap(_suggestionStrings.Length)].Length - 2){
            _autoCompleteLabel.Text = string.Empty;
        }
    }

    private void UpdateAutoCompleteSuggestionsLabel(){
        StringBuilder b = new();
        for (int i = _suggestionStrings.Length - 1; i > -1; i--){
            b.Append($"[url={_suggestionStrings[i]}|{_descriptions[_suggestionStrings[i]]}]");

            if (i == _currentShift.Wrap(_suggestionStrings.Length)){
                b.Append("[bgcolor=333333]");
            }

            string[] methodParts = _suggestionStrings[i].Split('(')[0].Split('.');
            for (int j = 0; j < methodParts.Length; j++){
                if (j == methodParts.Length - 1){
                    b.Append(
                        $"[color={DCColorTheme.Suggestions["Method"]}]{methodParts[j]}[/color](");
                }
                else{
                    b.Append($"{methodParts[j]}.");
                }
            }

            b.Append($"{_commands[_suggestionStrings[i]]})");
            if (i == _currentShift.Wrap(_suggestionStrings.Length)){
                b.Append("[/bgcolor]");
            }

            b.Append("[/url]");
            if (i > 0){
                b.Append('\n');
            }
        }

        string formattedString = b.ToString();
        foreach (KeyValuePair<string, string> kvp in DCExtensions.TypeReplacementStrings){
            string replace = formattedString.Replace(kvp.Key,
                $"[color={DCColorTheme.Suggestions["Type"]}]{kvp.Value}[/color]");
            formattedString = replace;
        }

        _autoCompleteSuggestions.Size = Vector2.Zero;
        _autoCompleteSuggestions.Text = formattedString;
        _autoCompleteSuggestions.Size = new Vector2(_autoCompleteSuggestions.Size.X, _suggestionStrings.Length * 24);
    }

    private void UpdateAutoCompleteSuggestionStrings(){
        if (_commandInput.Text.StartsWith('(')) return;
        _suggestionStrings = _commandInput.Text.Length > 0
            ? _commands.Keys.Where(s => s.ToLower().StartsWith(_commandInput.Text.ToLower().Split('(')[0])).ToArray()
            : Array.Empty<string>();
        if (_suggestionStrings.Length > DC.Instance.MaxAutocompleteLines)
            Array.Resize(ref _suggestionStrings, DC.Instance.MaxAutocompleteLines);
        _autoCompleteSuggestions.Visible = _suggestionStrings.Length > 0;
    }

    private void PopHistory(){
        _commandInput.Text = CommandInterpreter.LastCommand;
        CallDeferred(nameof(InputCaretToEnd), 0);
    }

    private void ShiftSuggestions(int dir){
        _currentShift += dir;
        CallDeferred(nameof(InputCaretToEnd), 0);
    }

    private void InputCaretToEnd(int offset = 0){
        _commandInput.CaretColumn = _commandInput.Text.Length + offset;
    }

    private void UpdateAutocomplete(){
        UpdateAutoCompleteSuggestionStrings();
        UpdateAutoCompleteLabel();
        UpdateAutoCompleteSuggestionsLabel();
    }

    private bool TryCorrectCase(InputEventKey k){
        if (_suggestionStrings.Length == 0) return false;
        if (!k.Keycode.IsLetter()) return false;
        if (k.CtrlPressed || k.AltPressed) return false;
        ReadOnlySpan<char> suggestionChars = _suggestionStrings[0];
        char[] inputChars = _commandInput.Text.ToCharArray();
        for (int i = 0; i < inputChars.Length; i++){
            if (suggestionChars.Length - 1 < i) break;
            if (i > _suggestionStrings[0].Length - 2) break;
            inputChars[i] = suggestionChars[i];
        }

        if (_commandInput.Text.Length > _suggestionStrings[0].Length - 2) return false;
        ReadOnlySpan<char> rOChars = inputChars;
        _commandInput.Text = rOChars.ToString();
        _commandInput.CaretColumn = _commandInput.Text.Length;
        return true;
    }

    public void SetCommandPromptFromCommandTree(string s){
        _commandInput.Text = $"{s}()";
        UpdateAutocomplete();
        _commandInput.GrabFocus();
        CallDeferred(nameof(InputCaretToEnd), -1);
    }
}