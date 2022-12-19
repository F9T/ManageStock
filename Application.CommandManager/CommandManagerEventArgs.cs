using Application.CommandManager.Commands;
using System;

namespace Application.CommandManager
{

    public class CommandManagerEventArgs : EventArgs
    {
        public CommandManagerEventArgs(CommandGroup _Commands) => this.Commands = _Commands;

        public EnumCommandActionType CommandActionType { get; set; }

        public CommandGroup Commands { get; set; }
    }
}
