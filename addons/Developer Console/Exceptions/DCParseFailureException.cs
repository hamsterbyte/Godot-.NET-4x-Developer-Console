using System;

namespace hamsterbyte.DeveloperConsole;

public class DCParseFailureException : DCException{
    public DCParseFailureException(Type t){
        _message = $"{t} has no method to parse from string";
    }
}