using Godot;
using hamsterbyte.DeveloperConsole;

public partial class NonStaticCommandTest : Control{
    [ConsoleCommand]
    private void NonStaticCommand(){
        DC.Print("Non-Static Command Executed");
    }
}