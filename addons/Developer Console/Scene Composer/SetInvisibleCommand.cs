using Godot;

namespace hamsterbyte.DeveloperConsole.Composition;

public class SetInvisibleCommand : ICompositionCommand{
    private readonly SubViewportContainer _subViewportContainer;
    private readonly bool _previousVisibility;

    public SetInvisibleCommand(SubViewportContainer container){
        _subViewportContainer = container;
        _previousVisibility = container.Visible;
    }

    public void Execute(){
        if (_subViewportContainer is null) return;
        _subViewportContainer.Visible = false;
        DC.Print($"{_subViewportContainer.Name} visibility set to: false");
    }

    public void Undo(){
        if (_subViewportContainer is null) return;
        _subViewportContainer.Visible = _previousVisibility;
        DC.Print($"{_subViewportContainer.Name} visibility set to: {_previousVisibility}");
    }
}