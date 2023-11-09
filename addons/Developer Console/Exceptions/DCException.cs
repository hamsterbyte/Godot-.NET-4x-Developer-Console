using System;

namespace hamsterbyte.DeveloperConsole;

public class DCException : Exception{
    protected string _message;
    public override string Message => _message;

    protected DCException(){
    }

    public DCException(string message){
        _message = message;
    }
}