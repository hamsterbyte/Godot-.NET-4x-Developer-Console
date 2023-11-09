using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static partial class CommandInterpreter{
    public static (Node currentNode, Dictionary<string, MethodInfo> commands) Context =>
        (_currentNode, _commands);

    public static Node CurrentNode => _currentNode;
    private static Node _currentNode;
    private static string _lastCommand;
    public static string LastCommand => _lastCommand;
    public static bool Busy{ get; private set; }

    #region INITIALIZE

    public static void Initialize(){
        bool success = TryInitialize();
        DC.Print($"Command Interpreter => {success.OKFail()}");
        if (success) DCCrosshair.Initialize();
    }

    private static bool TryInitialize(){
        try{
            DC.SetSuppressionLevel(DC.PrintSuppression.All);
            ChangeContext("/root");
            GetStaticCommands(out _staticCommands);
            _commands = _staticCommands;
            GetInstanceCommands(out _instanceCommands, _currentNode);
            DC.SetSuppressionLevel(DC.Instance.PrintSuppression);
            return true;
        }
        catch (Exception e){
            e.PrintToDC();
            return false;
        }
    }

    #endregion

    #region INTERPRET

    public static async Task Interpret(string commandString){
        try{
            if (commandString == "") return;
            _lastCommand = commandString;
            if (TryShortcuts(commandString)) return;
            if (TryMath(commandString)) return;
            ConsoleCommand command = ParseConsoleCommand(commandString);
            if (command is not null) await ExecuteConsoleCommand(command);
        }
        catch (Exception e){
            HandleDCError(e);
        }
        finally{
            Command.ResetToken();
        }
    }

    private static bool TryShortcuts(string commandString){
        for (int i = 0; i < _shortcuts.Length; i++){
            if (!commandString.StartsWith(_shortcuts[i].Character)) continue;
            _shortcuts[i].Command.Invoke(commandString);
            return true;
        }

        return false;
    }


    private static readonly char[] _validExpressionStartChars ={ '(', '-' };

    private static bool TryMath(string commandString){
        if (!char.IsNumber(commandString[0]) && !_validExpressionStartChars.Contains(commandString[0])) return false;
        double result = Convert.ToDouble(new DataTable().Compute(commandString, null));
        DC.Print(result);
        return true;
    }

    private static async Task ExecuteConsoleCommand(ConsoleCommand command){
        CommandReturnType returnType = GetCommandReturnType(command.Method);
        if ((int)returnType <= 1){
            // If not a task, execute without waiting and print result if it's not void
            ExecuteAndPrintResult(command);
        }
        else{
            // If it's a task, execute and await it, then print result if it has one
            await ExecuteAndPrintResultAsync(command, returnType);
        }
    }

    private static void ExecuteAndPrintResult(ConsoleCommand command){
        Type returnType = command.Method.ReturnType;
        dynamic result = command.Method.Invoke(_currentNode, command.Params);

        if (returnType != typeof(void)){
            DC.Print(result);
        }
    }

    

    private static async Task ExecuteAndPrintResultAsync(ConsoleCommand command, CommandReturnType returnType){
        dynamic task = command.Method.Invoke(_currentNode, command.Params);
        if (task is not null){
            DC.Print("Processing...");
            await task;
            dynamic result = task.GetType().GetProperty("Result")?.GetValue(task, null);
            if (result is not null && returnType == CommandReturnType.TaskWithResult){
                DC.Print(result);
            }
        }
        else{
            throw new DCInvalidCommandException(command.Method.Name);
        }
    }

    private static ConsoleCommand ParseConsoleCommand(string commandString){
        return ConsoleCommand.FromString(commandString, _commands);
    }

    private static void HandleDCError(Exception e){
        while (e.InnerException is not null){
            e = e.InnerException;
        }

        e.PrintToDC();
    }

    #endregion

    #region COMMANDS

    private static Dictionary<string, MethodInfo> _commands = new();
    public static Dictionary<string, MethodInfo> Commands => _commands;
    public static DCDelegates.onConsoleCommandsUpdated OnGetStaticCommands;

    private static Dictionary<string, MethodInfo> _instanceCommands = new();
    public static Dictionary<string, MethodInfo> InstanceCommands => _instanceCommands;
    public static DCDelegates.onConsoleCommandsUpdated OnGetInstanceCommands;

    private static Dictionary<string, MethodInfo> _staticCommands = new();
    public static Dictionary<string, MethodInfo> StaticCommands => _staticCommands;

    private static void GetStaticCommands(out Dictionary<string, MethodInfo> commands){
        commands = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                .Where(method => method.GetCustomAttributes(typeof(ConsoleCommandAttribute), true).Any()))
            .OrderBy(method => {
                ConsoleCommandAttribute commandAttribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
                return commandAttribute?.Prefix ?? string.Empty;
            })
            .ThenBy(method => method.Name)
            .ToDictionary(method => {
                ConsoleCommandAttribute commandAttribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
                string prefix = commandAttribute?.Prefix != string.Empty
                    ? commandAttribute?.Prefix + '.'
                    : string.Empty;
                return $"{prefix}{method.Name}";
            });

        OnGetStaticCommands?.Invoke(commands);
    }

    private static void GetInstanceCommands(out Dictionary<string, MethodInfo> commands, object targetObject){
        commands = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes()
                .Where(type => type == targetObject.GetType()))
            .SelectMany(type => type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(method => method.GetCustomAttributes(typeof(ConsoleCommandAttribute), true).Any()))
            .OrderBy(method => method.Name)
            .ToDictionary(method => {
                ConsoleCommandAttribute commandAttribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
                string prefix = commandAttribute?.Prefix != string.Empty
                    ? commandAttribute?.Prefix + '.'
                    : string.Empty;
                return $"{prefix}{targetObject.GetType().BaselessString()}.{method.Name}";
            });

        OnGetInstanceCommands?.Invoke(commands);
    }

    private static void ResetContextCommands(){
        foreach (string key in _instanceCommands.Keys){
            _commands.Remove(key);
        }

        _instanceCommands.Clear();
    }

    private static void MergeConsoleCommands(){
        foreach (KeyValuePair<string, MethodInfo> commandPair in _instanceCommands){
            _commands.TryAdd(commandPair.Key, commandPair.Value);
        }

        OnConsoleCommandsUpdated?.Invoke(_commands);
    }

    public static DCDelegates.onConsoleCommandsUpdated OnConsoleCommandsUpdated;

    private static void UpdateContextCommands(Node selectedNode){
        ResetContextCommands();
        GetInstanceCommands(out _instanceCommands, selectedNode);
        MergeConsoleCommands();
    }

    #endregion

    private static CommandReturnType GetCommandReturnType(MethodInfo methodInfo){
        Type returnType = methodInfo.ReturnType;

        if (returnType == typeof(void)) return CommandReturnType.None;
        if (returnType == typeof(Task)) return CommandReturnType.Task;
        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            return CommandReturnType.TaskWithResult;
        return CommandReturnType.Type;
    }

    private enum CommandReturnType{
        None,
        Type,
        Task,
        TaskWithResult
    }
}