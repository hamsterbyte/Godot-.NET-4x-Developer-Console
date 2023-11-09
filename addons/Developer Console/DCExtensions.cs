using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static partial class DCExtensions{
    public static readonly Dictionary<string, string> TypeReplacementStrings = new(){
        { "Boolean", "bool" },
        { "Byte", "byte" },
        { "SByte", "sbyte" },
        { "Int16", "short" },
        { "UInt16", "ushort" },
        { "Int32", "int" },
        { "UInt32", "uint" },
        { "Int64", "long" },
        { "UInt64", "ulong" },
        { "Char", "char" },
        { "Double", "double" },
        { "Single", "float" },
        { "String", "string" }
    };

    public static string BaselessString(this Type t){
        string[] types = t.ToString().Split('.');
        return types[^1];
    }

    public static bool IsLetter(this Key k){
        int value = (int)k;
        return value is > 64 and < 91;
    }

    public static string ParamsAsString(this MethodInfo m){
        ParameterInfo[] infos = m.GetParameters();

        if (infos.Length == 0)
            return string.Empty;

        return string.Join(", ", infos.Select(info =>
            $"{info.ParameterType.BaselessString()} {info.Name}"));
    }

    public static string SetWidth(this string s, int desiredWidth){
        return $"{s.PadRight(desiredWidth - s.Length)}\t";
    }

    public static int Wrap(this int value, int max){
        int remainder = value % Math.Max(max, 1);
        return remainder < 0 ? max + remainder : remainder;
    }

    public static string GetPath(this TreeItem treeItem){
        Stack<string> pathComponents = new Stack<string>();

        while (treeItem.GetParent() is not null){
            pathComponents.Push(treeItem.GetText(0));
            treeItem = treeItem.GetParent();
        }

        pathComponents.Push("root");

        return "/" + string.Join("/", pathComponents);
    }

    public static async Task InvokeDelayed(this Delegate d, int milliseconds, params object[] objects){
        await Task.Delay(milliseconds);
        d.DynamicInvoke(objects);
    }


    /// <summary>
    /// Used to invoke a callback after 2 frames
    /// Waiting for two frames ensures that any deferred calls from the frame of this invocation will have been executed
    /// </summary>
    /// <param name="d">Delegate to invoke</param>
    /// <param name="objects">Parameters</param>
    public static async Task InvokeAwaited(this Delegate d, params object[] objects){
        double currentFrame = Engine.GetProcessFrames();
        while (Engine.GetProcessFrames() < currentFrame + 2){
            await Task.Delay(1);
        }

        d.DynamicInvoke(objects);
    }

    /// <summary>
    /// Used to invoke a method after a certain number of frames
    /// By default it will wait for 2 frames to ensure that all deferred calls have been executed
    /// </summary>
    /// <param name="d"></param>
    /// <param name="frames"></param>
    /// <param name="objects"></param>
    public static async Task InvokeAwaited(this Delegate d, int frames, params object[] objects){
        double currentFrame = Engine.GetProcessFrames();
        while (Engine.GetProcessFrames() < currentFrame + frames){
            await Task.Delay(1);
        }

        d.DynamicInvoke(objects);
    }

    public static string OKFail(this bool b) => b ? "Ok" : "Fail";

    public static string PromptString(this Node n) => $"${n.GetPath().ToString()?.TrimStart('/')}::";

    public static void PrintToDC(this Exception e){
        DC.PrintError($"{e.GetType().BaselessString()}: {e.Message}");
    }
}