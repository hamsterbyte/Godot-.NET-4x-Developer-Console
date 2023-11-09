using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace hamsterbyte.DeveloperConsole;

public partial class CommandInterpreter{
    private static void GetHelp(string commandString){
        if (commandString.Length < 2){
            Help(_commands);
        }
        else{
            if (commandString[1] == '?'){
                Help(_instanceCommands);
            }
            else{
                Help(_commands, commandString.TrimStart('?'));
            }
        }
    }
    
    private static void Help(Dictionary<string, MethodInfo> commands, string searchString = ""){
        foreach (KeyValuePair<string, MethodInfo> c in commands){
            string description = c.Value.GetCustomAttribute<ConsoleCommandAttribute>()?.Description;
            if (!c.Key.ToLower().Contains(searchString.ToLower()) && !description!.ToLower().Contains(searchString.ToLower())) continue;
            int line = DC.Instance.LastOutputLineIndex;
            ParameterInfo[] infos = c.Value.GetParameters();
            StringBuilder b = new();
            b.Append($"{c.Key}(");
            for (int i = 0; i < infos.Length; i++){
                b.Append($"{(i == 0 ? string.Empty : ", ")}{infos[i].ParameterType.BaselessString()} {infos[i].Name}");
            }

            b.Append(')');
            string formattedString = b.ToString();
            foreach (KeyValuePair<string, string> kvp in DCExtensions.TypeReplacementStrings){
                string replace = formattedString.Replace(kvp.Key, kvp.Value);
                formattedString = replace;
            }

            DC.Print(formattedString);
            if (description == string.Empty) continue;
            DC.IncreaseIndent();
            DC.Print($"/* {description} */");
            DC.DecreaseIndent();
            DC.Instance.CallDeferred(nameof(DC.Instance.FoldLine), line);
        }
    }
}