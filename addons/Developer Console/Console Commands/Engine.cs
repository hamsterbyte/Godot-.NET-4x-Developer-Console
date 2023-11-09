using System.Threading.Tasks;
using Godot;

namespace hamsterbyte.DeveloperConsole;

public static partial class DCEngine{
    #region GET

    [ConsoleCommand(Prefix = "Engine", Description = "Print current fps")]
    private static double GetFPS() => Engine.GetFramesPerSecond();

    [ConsoleCommand(Prefix = "Engine", Description = "Print number of frames drawn since init")]
    private static int GetFramesDrawn() => Engine.GetFramesDrawn();

    [ConsoleCommand(Prefix = "Engine", Description = "Print number of physics frames since init")]
    private static ulong GetPhysicsFrames() => Engine.GetPhysicsFrames();

    [ConsoleCommand(Prefix = "Engine", Description = "Print number of process frames since init")]
    private static ulong GetProcessFrames() => Engine.GetProcessFrames();

    [ConsoleCommand(Prefix = "Engine", Description = "Print current engine time scale")]
    private static double GetTimeScale() => Engine.TimeScale;

    [ConsoleCommand(Prefix = "Engine", Description = "Print max fps")]
    private static int GetMaxFPS() => Engine.MaxFps;

    [ConsoleCommand(Prefix = "Engine", Description = "Print physics jitter fix")]
    private static double GetPhysicsJitterFix() => Engine.PhysicsJitterFix;

    [ConsoleCommand(Prefix = "Engine", Description = "Print physics ticks per second")]
    private static int GetPhysicsTicksPerSecond() => Engine.PhysicsTicksPerSecond;

    [ConsoleCommand(Prefix = "Engine", Description = "Print max physics steps per frame")]
    private static int GetMaxPhysicsStepsPerFrame() => Engine.MaxPhysicsStepsPerFrame;

    #endregion

    #region SET

    [ConsoleCommand(Prefix = "Engine", Description = "Set the current engine time scale")]
    private static double SetTimeScale(double timeScale){
        Engine.TimeScale = timeScale;
        return GetTimeScale();
    }

    [ConsoleCommand(Prefix = "Engine", Description = "Set the physics jitter fix")]
    private static double SetPhysicsJitterFix(double physicsJitterFix){
        Engine.PhysicsJitterFix = physicsJitterFix;
        return GetPhysicsJitterFix();
    }

    [ConsoleCommand(Prefix = "Engine", Description = "Suppress errors from being output to Godot console")]
    private static bool SuppressGDErrorMessages(bool suppress){
        Engine.PrintErrorMessages = !suppress;
        return !Engine.PrintErrorMessages;
    }

    [ConsoleCommand(Prefix = "Engine", Description = "Set physics ticks per second")]
    private static int SetPhysicsTicksPerSecond(int ticks){
        Engine.PhysicsTicksPerSecond = ticks;
        return GetPhysicsTicksPerSecond();
    }

    [ConsoleCommand(Prefix = "Engine", Description = "Set max physics steps per frame")]
    private static int SetMaxPhysicsStepsPerFrame(int maxSteps){
        Engine.MaxPhysicsStepsPerFrame = maxSteps;
        return GetMaxPhysicsStepsPerFrame();
    }

    [ConsoleCommand(Prefix = "Engine", Description = "Set max fps, a value of 0 means unlimited")]
    private static int SetMaxFPS(int maxFPS){
        Engine.MaxFps = maxFPS;
        return GetMaxFPS();
    }

    #endregion
}