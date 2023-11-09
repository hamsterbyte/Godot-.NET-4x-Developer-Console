using System;
using System.Collections.Generic;
using System.Reflection;

namespace hamsterbyte.DeveloperConsole;

public class ConsoleCommand{
    public MethodInfo Method;
    public object[] Params;

    public override string ToString() => $"{Method.Name} ({Params?.Length ?? 0})";


    private ConsoleCommand(MethodInfo method, object[] parameters){
        Method = method;
        Params = parameters;
    }


    public static ConsoleCommand FromString(string commandString, Dictionary<string, MethodInfo> commands){
        if (string.IsNullOrEmpty(commandString)){
            throw new DCException("Cannot parse an empty command string.");
        }

        string[] decomposedCommand = DecomposeCommandString(commandString);

        if (!commands.TryGetValue(decomposedCommand[0], out MethodInfo method)){
            throw new DCInvalidCommandException(decomposedCommand[0]);
        }

        ParameterInfo[] commandParameterInfos = method.GetParameters();
        ValidateParameterCount(commandParameterInfos.Length, decomposedCommand.Length - 1, method.Name);

        object[] parameters = ParseParameters(decomposedCommand, commandParameterInfos);

        return new ConsoleCommand(method, parameters);
    }

    private static string[] DecomposeCommandString(string commandString){
        return commandString.Split(new[]{ '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static void ValidateParameterCount(int expectedCount, int actualCount, string methodName){
        if (expectedCount != actualCount){
            throw new DCParameterMismatchException(methodName, expectedCount);
        }
    }

    private static object[] ParseParameters(string[] decomposedCommand, ParameterInfo[] commandParameterInfos){
        object[] parameters = new object[commandParameterInfos.Length];

        for (int i = 1; i < decomposedCommand.Length; i++){
            Type parameterType = commandParameterInfos[i - 1].ParameterType;
            string parameterString = decomposedCommand[i].Trim();

            if (TryParseParameter(parameterType, parameterString, out object parsedValue)){
                parameters[i - 1] = parsedValue;
            }
            else{
                throw new DCInvalidParameterFormatException(parameterString, parameterType);
            }
        }

        return parameters;
    }

    private static bool TryParseParameter(Type parameterType, string parameterString, out object parsedValue){
        if (parameterType == typeof(string)){
            parsedValue = parameterString;
            return true;
        }

        MethodInfo parseMethod = parameterType.GetMethod("Parse", new[]{ typeof(string) });

        if (parseMethod is not null){
            try{
                parsedValue = parseMethod.Invoke(null, new object[]{ parameterString });
                return true;
            }
            catch{
                throw new DCParseFailureException(parameterType);
            }
        }

        parsedValue = null;
        return false;
    }
}