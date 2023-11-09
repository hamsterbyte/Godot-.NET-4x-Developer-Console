namespace hamsterbyte.DeveloperConsole;

public partial class DC{
    /// <summary>
    /// Quit the game
    /// </summary>
    [ConsoleCommand(Prefix = "Application", Description = "Quit the game")]
    private static void Quit(){
        Instance.GetTree().Quit();
    }
}