using System.Threading;

namespace hamsterbyte.DeveloperConsole;

public static class Command {
    
    private static CancellationTokenSource _cancellationTokenSource = new();
    public static CancellationToken Token => _cancellationTokenSource.Token;
    
    [ConsoleCommand(Prefix = "Command", 
        Description = "Cancel all running console commands that use the command token.")]
    private static void Cancel(){
        _cancellationTokenSource.Cancel();
    }

    public static void ResetToken(){
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
    }
    
}