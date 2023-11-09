using Godot;
using hamsterbyte.DeveloperConsole;

public partial class DeveloperConsoleUI : CanvasLayer{
    #region VARIABLES

    //CONTROLS---------------------------------------
    [ExportGroup("Controls")] [Export] private CodeEdit _output;
    //-----------------------------------------------

    //SETTINGS---------------------------------------
    [ExportGroup("Settings")] [Export(PropertyHint.Enum)]
    public DCAccessLevel AccessLevel = DCAccessLevel.WithArg;

    [Export(PropertyHint.Enum)] public DC.PrintSuppression PrintSuppression = DC.PrintSuppression.GD;
    [Export(PropertyHint.Range, "3,10")] public int MaxAutocompleteLines{ get; private set; } = 10;
    [Export] public bool PrintLicense = true;
    //-----------------------------------------------

    public int LastOutputLineIndex => _output.GetLineCount() - 1;
    private Input.MouseModeEnum _defaultMouseMode;

    #endregion

    public override void _Ready(){
        InitializeSystem();
    }

    private void InitializeSystem(){
        DC.Initialize(this);
    }

    public void InitializeUI(){
        DC.PrintComment(" ----------------- Initializing User Interface ----------------- ");
        PropagateCall("Initialize", null, true);
        DC.PrintComment(" ----------------- Initialization Complete ----------------- ");
        DC.SetSuppressionLevel(PrintSuppression);
        DC.ChangeContext("/root");
    }


    private void ResetAll(){
        //InitializeReset();
    }

    private void InitializeReset(){
        /*
        DC.Submit("Output.Clear()");
        DC.PrintComment(" ---------------------- Resetting System ---------------------- ");
        DC.Reset();
        DC.PrintComment(" ----------------- Resetting User Interface ----------------- ");
        PropagateCall("Reset", null, true);
        */
    }

    #region INPUT

    public override void _Input(InputEvent @event){
        if (@event is not InputEventKey key) return;
        CheckInput(key);
    }

    private void CheckInput(InputEventKey key){
        if (!key.Pressed) return;
        switch (key.Keycode){
            case Key.Quoteleft:
                ToggleConsole();
                break;
            case Key.Escape:
                if (!Visible) return;
                ToggleConsole();
                break;
        }
    }

    #endregion

    private void ToggleConsole(){
        Visible = !Visible;
        if (Visible){
            _defaultMouseMode = Input.MouseMode;
            Input.MouseMode = Input.MouseModeEnum.Visible;
            foreach (StringName s in InputMap.GetActions()){
                if (s.ToString()!.Contains("ui_")) continue;
                InputMap.ActionEraseEvents(s);
            }
        }
        else{
            Input.MouseMode = _defaultMouseMode;
            InputMap.LoadFromProjectSettings();
        }

        DC.SetEnabled(Visible);
    }

    public void FoldLine(int line){
        _output.FoldLine(line);
    }
}