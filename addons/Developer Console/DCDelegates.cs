using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public abstract class DCDelegates{
    public delegate void onConsoleCommandsUpdated(Dictionary<string, MethodInfo> commands);

    public delegate void onPrint(string message);

    public delegate void onCallback();

    public delegate void onPrefabInstantiated(Node n);

    public delegate void onContextChanged((Node, Dictionary<string, MethodInfo>) context);

    public delegate void onCommandSubmitted(string commandString);

    public delegate void onConsoleToggled(bool toggled);

    public delegate void onModeChanged(int mode);

    public delegate void onToggle(bool b);

}