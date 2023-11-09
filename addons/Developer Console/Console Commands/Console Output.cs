namespace hamsterbyte.DeveloperConsole;

public static partial class OutputCommands{
    /// <summary>
    /// Clear the output console
    /// </summary>
    [ConsoleCommand(Prefix = "Output", Description = "Clear all output and command history from the console")]
    private static void Clear(){
        DC.OnClear?.Invoke();
    }
}