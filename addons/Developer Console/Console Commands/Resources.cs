using System.Collections.Generic;
using System.Linq;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static class Resources{
    public static void GetAllDirectories(string path){
        if (DirAccess.DirExistsAbsolute(path)){
            foreach (string dir in DirAccess.Open(path).GetDirectories()){
                DC.Print($"{path}/{dir}");
                DC.IncreaseIndent();
                GetAllDirectories($"{path}/{dir}");
                DC.DecreaseIndent();
            }
        }
        else{
            throw new DCException($"Requested directory '{path}' does not exist");
        }
    }

    private static readonly List<string> LastFilesList = new();

    public static void GetFiles(string path){
        if (DirAccess.DirExistsAbsolute(path)){
            string[] fileNames = DirAccess.Open($"{path}").GetFiles();
            for (int i = 0; i < fileNames.Length; i++){
                if (fileNames[i].EndsWith(".remap")){
                    fileNames[i] = fileNames[i].TrimEnd(".remap".ToCharArray());
                }

                LastFilesList.Add($"{path}/{fileNames[i]}");
            }

            foreach (string dir in DirAccess.Open(path).GetDirectories()){
                GetFiles($"{path}/{dir}");
            }
        }
        else{
            throw new DCException($"Requested directory '{path}' does not exist");
        }
    }

    public static void GetResourceInformation(string path, ref List<ResourceInformation> resourceInformations){
        char[] splitChars ={ '.', '/' };
        LastFilesList.Clear();
        resourceInformations.Clear();
        GetFiles(path);
        resourceInformations.AddRange(Resources.LastFilesList.Select(t =>
            new ResourceInformation(t.Split(splitChars)[^2], t)));
    }
}

public readonly struct ResourceInformation{
    public readonly string Name;
    public readonly string Path;

    public ResourceInformation(string name, string path){
        Name = name;
        Path = path;
    }

    public override string ToString(){
        return $"Name:: {Name.SetWidth(64)}Path:: {Path.SetWidth(200)}";
    }
}