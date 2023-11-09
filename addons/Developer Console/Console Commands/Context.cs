using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace hamsterbyte.DeveloperConsole{
    public partial class DC{
        /// <summary>
        /// Print paths to immediate children of context
        /// </summary>
        [ConsoleCommand(Prefix = "Context", Description = "Print a list of immediate children in the current context")]
        private static void GetChildren(){
            if (CurrentNode.GetChildCount() > 0){
                foreach (Node node in CurrentNode.GetChildren()){
                    if (node.Name == "DeveloperConsole") continue;
                    Print(node.GetPath());
                }
            }
            else{
                throw new DCException("Context contains no children.");
            }
        }

        /// <summary>
        /// Print all paths to all children in context, includes internal
        /// </summary>
        [ConsoleCommand(Prefix = "Context",
            Description = "Print a list of all children in the current context. Includes internal")]
        private static void GetAllChildren(){
            if (CurrentNode.GetChildCount() > 0){
                GetChildren(CurrentNode);
            }
            else{
                throw new DCException("Context contains no children");
            }
        }

        /// <summary>
        /// Print all node paths in the scene tree
        /// </summary>
        [ConsoleCommand(Prefix = "Context", Description = "Print a list of all nodes in the tree")]
        private static void GetTree(){
            if (Instance.GetTree().Root.GetChildCount() != 0){
                GetChildren(Instance.GetTree().Root);
            }
            else{
                throw new DCException("Tree contains no children");
            }
        }

        private static void GetChildren(Node targetNode){
            if (targetNode.GetChildCount() <= 0) return;
            foreach (Node node in targetNode.GetChildren()){
                if (node.GetPath().ToString()!.Contains("DeveloperConsole")) continue;
                Print(node.GetPath());
                IncreaseIndent();
                GetChildren(node);
                DecreaseIndent();
            }
        }

        /// <summary>
        /// Recursively search children of the current node for a child with a given name
        /// If found, set current node to search result
        /// </summary>
        /// <param name="name">Name of the node. Case Sensitive</param>
        [ConsoleCommand(Prefix = "Context", Description =
            "Search recursively by name through children in the current context. Print all found nodes to console.")]
        private static void FindChild(string name){
            if (FindIn(name, CurrentNode).Count == 0)
                throw new DCException($"Node with name '{name}' does not exist in the current context.");
        }

        /// <summary>
        /// Recursively search entire scene tree for a node with a given name
        /// If found, set current node to search result
        /// Prefer using FindChild for performance
        /// </summary>
        /// <param name="name">Name of the node. Case Sensitive</param>
        [ConsoleCommand(Prefix = "Context", Description =
            "Search recursively by name through all nodes in scene tree. Print all found nodes to console")]
        private static void Find(string name){
            if (FindIn(name, Instance.GetTree().Root).Count == 0)
                throw new DCException($"Node with name '{name}' does not exist in the scene tree.");
        }

        private static List<string> FindIn(string name, Node context){
            Queue<Node> _treeQueue = new();
            List<string> foundNodes = new();
            Node node;
            for (int i = 0; i < context.GetChildren().Count; i++){
                node = context.GetChildren()[i];
                if (node.Name == "DeveloperConsole") continue;
                _treeQueue.Enqueue(node);
            }

            while (_treeQueue.Count > 0){
                node = _treeQueue.Dequeue();
                if (node.Name.ToString()!.ToLower().Contains(name.ToLower())){
                    foundNodes.Add(node.GetPath());
                }

                foreach (Node child in node.GetChildren()){
                    _treeQueue.Enqueue(child);
                }
            }

            if (foundNodes.Count <= 0) return foundNodes;
            {
                for (int i = 0; i < foundNodes.Count; i++){
                    Print($"{foundNodes[i]}");
                }

                if (foundNodes.Count == 1){
                    ChangeContext(foundNodes[0]);
                }
            }

            return foundNodes;
        }
    }
}