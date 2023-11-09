using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace hamsterbyte.DeveloperConsole{
    public static partial class DC{
        public static DCDelegates.onCommandSubmitted OnCommandSubmitted;
        public static DCDelegates.onPrint OnPrint;
        public static DCDelegates.onModeChanged OnModeChanged;
        public static DCDelegates.onCallback OnClear;
        public static DCDelegates.onCallback OnCommandCompleted;
        public static DCDelegates.onCallback OnEnable;
        public static DCDelegates.onCallback OnDisable;
        public static DCDelegates.onToggle OnToggle;
        public static DCDelegates.onCallback OnInitialized;
        public static int Mode{ get; private set; }

        public static Node Root => Instance.GetTree().Root;
        public static DeveloperConsoleUI Instance{ get; private set; }
        public static bool Enabled{ get; private set; }

        #region WRAPPERS

        public static Node CurrentNode => CommandInterpreter.CurrentNode;
        public static (Node CurrentNode, Dictionary<string, MethodInfo> Commands) Context => CommandInterpreter.Context;

        public static void ChangeContext(string path) => CommandInterpreter.ChangeContext(path);

        #endregion

        #region INITIALIZE

        public static void Initialize(DeveloperConsoleUI instance){
            DC.SetSuppressionLevel(PrintSuppression.GD);
            bool success = TryInitialize(instance);
            PrintComment(" ------------------ Initializing System ------------------ ");
            Print($"Developer Console => {success.OKFail()}");
            if (success) CommandInterpreter.Initialize();
        }

        private static bool TryInitialize(DeveloperConsoleUI instance){
            try{
                Instance = instance;
                if (!CheckDebugEnabled()){
                    Instance.QueueFree();
                    return false;
                }

                InitializeCallbacks();
                if (instance.PrintLicense){
                    SetSuppressionLevel(PrintSuppression.GD);
                    foreach (string s in License.Text){
                        Print(s);
                    }

                    SetSuppressionLevel(Instance.PrintSuppression);
                }

                OnInitialized?.Invoke();
                return true;
            }
            catch (Exception e){
                e.PrintToDC();
                return false;
            }
        }

        private static bool CheckDebugEnabled(){
            return Instance.AccessLevel switch{
                DCAccessLevel.None => false,
                DCAccessLevel.Debug => OS.IsDebugBuild(),
                DCAccessLevel.WithArg => OS.IsDebugBuild() || OS.GetCmdlineArgs().Contains("-developer"),
                DCAccessLevel.All => true,
                _ => false
            };
        }

        private static void InitializeCallbacks(){
            CommandInterpreter.OnContextChanged += ChangeContext;
        }

        #endregion

        private static int previousMode;
        public static void SetEnabled(bool enabled){
            Enabled = enabled;
            if (enabled){
                ChangeMode(previousMode);
                OnEnable?.InvokeAwaited();
            }
            else{
                previousMode = Mode;
                ChangeMode(-1);
                OnDisable?.InvokeAwaited();
            }

            OnToggle?.InvokeAwaited(Enabled);
        }

        public static void ChangeMode(int mode){
            Mode = mode;
            OnModeChanged?.Invoke(mode);
        }

        private static void ChangeContext((Node node, Dictionary<string, MethodInfo> commands) context){
            SetIndentLevel(0);
            Print($"${context.node.GetPath().ToString()?.TrimStart('/')}::");
        }


        /// <summary>
        /// Parse the command string, if a valid command is found invoke it
        /// If the string is a path, update the current object to the path
        /// </summary>
        /// <param name="commandString"></param>
        public static async void Submit(string commandString){
            Print($"{CommandInterpreter.CurrentNode.PromptString()} {commandString}");
            OnCommandSubmitted?.Invoke(commandString);
            IncreaseIndent();
            await CommandInterpreter.Interpret(commandString);
            DecreaseIndent();
            OnCommandCompleted?.InvokeAwaited();
        }

        #region PRINT

        private static int _indentLevel;
        private static PrintSuppression _suppressionLevel;

        public enum PrintSuppression{
            None,
            DC,
            GD,
            All
        }

        public static void SetSuppressionLevel(PrintSuppression suppressionLevel){
            _suppressionLevel = suppressionLevel;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static void SetIndentLevel(int level){
            _indentLevel = Math.Clamp(level, 0, int.MaxValue);
        }

        public static void IncreaseIndent(){
            _indentLevel++;
        }

        public static void DecreaseIndent(){
            _indentLevel = Math.Max(0, _indentLevel - 1);
        }

        /// <summary>
        /// Print a line to the developer console
        /// </summary>
        /// <param name="o"></param>
        public static void Print(object o){
            if (Instance is null || _suppressionLevel == PrintSuppression.All){
                return; // Print nothing if the instance is null or all is suppressed
            }

            if (_suppressionLevel != PrintSuppression.GD){
                PrintOutputToGD(o);
            }

            if (_suppressionLevel != PrintSuppression.DC){
                PrintOutputToDC(o);
            }
        }

        public static void PrintError(object o){
            Print($"#! {o} !#");
        }

        public static void PrintComment(object o){
            Print($"/* {o} */");
        }

        private static void PrintOutputToDC(object o){
            string output = _indentLevel > 0
                ? new string('\t', _indentLevel) + o
                : o.ToString();
            OnPrint?.Invoke(output);
        }

        private static void PrintOutputToGD(object o){
            GD.Print(o.ToString());
        }

        #endregion
    }

    public enum DCAccessLevel{
        None,
        Debug,
        WithArg,
        All
    }
}