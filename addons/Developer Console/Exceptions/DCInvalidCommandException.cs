namespace hamsterbyte.DeveloperConsole;

public class DCInvalidCommandException : DCException{
    public DCInvalidCommandException(string commandString){
        _message =
            $"Command '{commandString}' not recognized. Type ? for a list of commands or ?? for context specific commands. Commands are case sensitive.";
    }
}