using Godot;

namespace hamsterbyte.DeveloperConsole;

public partial class DCCrosshairUI : Control, ICanInitialize{
    public void Initialize(){
        bool success = TryInitialize();
        DC.Print($"DCCrosshairUI => {success.OKFail()}");
    }

    public bool TryInitialize() => true;
}