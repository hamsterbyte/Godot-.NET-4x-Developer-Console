namespace hamsterbyte.DeveloperConsole;

public class DCParameterMismatchException : DCException{
    public DCParameterMismatchException(string methodName, int expectedCount){
        _message = $"{methodName} expects {expectedCount} parameters.";
    }
}