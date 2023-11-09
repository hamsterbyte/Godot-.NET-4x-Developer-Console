namespace hamsterbyte.DeveloperConsole{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ConsoleCommandAttribute : Attribute{
        public string Prefix = string.Empty;
        public string Description = string.Empty;
    }
}