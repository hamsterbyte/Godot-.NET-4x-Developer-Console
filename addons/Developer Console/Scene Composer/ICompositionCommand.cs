using Godot;

namespace hamsterbyte.DeveloperConsole.Composition;

public interface ICompositionCommand{
    public void Execute();
    public void Undo();
}