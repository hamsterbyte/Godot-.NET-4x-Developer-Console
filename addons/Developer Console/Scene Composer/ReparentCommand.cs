using Godot;

namespace hamsterbyte.DeveloperConsole.Composition;

public class ReparentCommand : ICompositionCommand{
    private readonly Node node;
    private readonly Node newParent;
    private readonly Node oldParent;

    public ReparentCommand(Node n){
        node = n;
        oldParent = node.GetParentOrNull<Node>();
        newParent = FindValidAncestor();
    }

    public void Execute(){
        if (node is null || newParent is null) return;
        node.Reparent(newParent);
        DC.Print($"{node.Name} reparented to {newParent.Name}");
    }

    public void Undo(){
        if (node is null || oldParent is null) return;
        node.Reparent(oldParent);
        DC.Print($"{node.Name} reparented to {oldParent.Name}");
    }

    private Node FindValidAncestor(){
        if (node is null) return null;
        Node ancestor = node.GetParentOrNull<Node>();
        while (ancestor is not null){
            if (ancestor is not SubViewport and not SubViewportContainer){
                return ancestor;
            }

            ancestor = ancestor.GetParentOrNull<Node>();
        }

        return DC.Root;
    }
}