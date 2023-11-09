using System;
using System.Reflection;
using Godot;

namespace hamsterbyte.DeveloperConsole{
    public abstract partial class Nodes{
        public delegate void onNodeDestroyed(Node n);

        public static onNodeDestroyed OnNodeDestroyed;

        #region GET

        [ConsoleCommand(Prefix = "Node", Description = "Print the global position of the currently selected node.")]
        private static Vector2 GetPosition(){
            PropertyInfo p = DC.CurrentNode.GetType().GetProperty("GlobalPosition");
            if (p is null){
                throw new DCException("This node has no global position property.");
            }
            return (Vector2)p.GetValue(DC.CurrentNode)!;
        }

        #endregion

        #region DESTROY

        [ConsoleCommand(Prefix = "Node", Description =
            "Destroy the node at the given path. Cannot Destroy /root. If node is current context, set context to parent node")]
        private static void DestroyAt(string path){
            if (path == "/root") throw new DCException("Cannot destroy root.");
            if (path == DC.CurrentNode.GetPath()){
                DC.ChangeContext(DC.CurrentNode.GetParent().GetPath());
            }

            Node n = DC.Root.GetNodeOrNull(path);
            OnNodeDestroyed?.Invoke(n);
            n.QueueFree();
            DC.Print($"Node destroyed at '{path}'");
        }

        [ConsoleCommand(Prefix = "Node", Description =
            "Destroy node at the current context. Cannot Destroy /root. Set current context to parent node")]
        private static void Destroy(){
            if (DC.CurrentNode.GetPath() == "/root") throw new DCException("Cannot destroy root.");
            Node t = DC.CurrentNode;
            DC.ChangeContext(t.GetParent().GetPath());
            OnNodeDestroyed?.Invoke(t);
            t.QueueFree();
            DC.Print($"Node destroyed at '{t.GetPath()}'");
        }

        #endregion

        #region MOVE

        [ConsoleCommand(Prefix = "Node", Description = "Move the current context node by the specified amount")]
        private static void Move(int x, int y){
            try{
                Vector2 move = new(x, y);
                switch (DC.CurrentNode){
                    case Control control:
                        control.GlobalPosition += move;
                        DC.Print(control.GlobalPosition);
                        break;
                    case Node2D node2D:
                        node2D.GlobalPosition += move;
                        DC.Print(node2D.GlobalPosition);
                        break;
                    default:
                        throw new DCException("Current context node is not a Control or Node2D");
                }
            }
            catch (Exception e){
                throw new DCException(e.Message);
            }
        }

        [ConsoleCommand(Prefix = "Node", Description = "Move the current context node to the specified position")]
        private static void SetPosition(int x, int y){
            try{
                Vector2 newPos = new(x, y);
                switch (DC.CurrentNode){
                    case Control control:
                        control.GlobalPosition = newPos - control.PivotOffset;
                        DC.Print(control.GlobalPosition);
                        break;
                    case Node2D node2D:
                        node2D.GlobalPosition = newPos;
                        DC.Print(node2D.GlobalPosition);
                        break;
                    default:
                        throw new DCException("Current context node is not a Control or Node2D");
                }
            }
            catch (Exception e){
                throw new DCException(e.Message);
            }
        }

        [ConsoleCommand(Prefix = "Node", Description = "Move the current context node to the mouse position")]
        private static void MoveToMouse(){
            try{
                switch (DC.CurrentNode){
                    case Control control:
                        control.GlobalPosition = control.GetGlobalMousePosition() - control.PivotOffset;
                        DC.Print(control.GlobalPosition);
                        break;
                    case Node2D node2D:
                        node2D.GlobalPosition = node2D.GetGlobalMousePosition();
                        DC.Print(node2D.GlobalPosition);
                        break;
                    default:
                        throw new DCException("Current context node is not a Control or Node2D");
                }
            }
            catch (Exception e){
                throw new DCException(e.Message);
            }
        }

        #endregion
    }
}