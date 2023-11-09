using System;

namespace hamsterbyte.DeveloperConsole;

public class DCInvalidParameterFormatException : DCException{
    public DCInvalidParameterFormatException(string parameterString, Type t){
        _message = $"Invalid parameter format {t}.Parse('{parameterString}')";
    }
}