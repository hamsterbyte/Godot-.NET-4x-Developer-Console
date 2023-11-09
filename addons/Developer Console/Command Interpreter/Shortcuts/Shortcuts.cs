using System;

namespace hamsterbyte.DeveloperConsole;

public partial class CommandInterpreter{
    private static readonly (char Character, Action<string> Command)[] _shortcuts ={
        new('/', ChangeContext),
        new('?', GetHelp)
    };
}