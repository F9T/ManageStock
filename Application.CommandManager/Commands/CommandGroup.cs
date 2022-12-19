using System.Collections.Generic;
using System.Linq;

namespace Application.CommandManager.Commands
{
    public class CommandGroup
    {
        public CommandGroup()
        {
            Commands = new List<CommandBase>();
        }

        public CommandGroup(CommandBase _Command) : this()
        {
            Commands.Add(_Command);
        }

        public IList<CommandBase> Commands { get; set; }

        public void Undo()
        {
            foreach (CommandBase commandBase in Commands.Reverse())
            {
                commandBase.Undo();
            }
        }

        public void Redo()
        {
            foreach (CommandBase command in Commands)
            {
                command.Redo();
            }
        }

        public bool IsUndoable => Commands.All(c => c.IsUndoable);
    }
}
