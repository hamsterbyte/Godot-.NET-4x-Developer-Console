using System.Threading.Tasks;

namespace hamsterbyte.DeveloperConsole;

public static class MyCustomCommands{
    [ConsoleCommand(Prefix = "Test", Description = "My test command")]
    private static void PrintToConsoleTest(string s){
        DC.Print(s);
    }

    [ConsoleCommand(Prefix = "Test")]
    private static int AddTest(int a, int b){
        return a + b;
    }

    [ConsoleCommand(Prefix = "Test")]
    private static async Task AsyncTest(){
        await Task.Delay(3000);
        DC.Print("Task Complete");
    }

    [ConsoleCommand(Prefix = "Test")]
    private static async Task<int> AsyncMultiply(int a, int b){
        await Task.Delay(3000);
        return a * b;
    }

}