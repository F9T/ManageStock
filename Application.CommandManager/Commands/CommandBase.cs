namespace Application.CommandManager.Commands
{
    public abstract class CommandBase
    {
        public object Context { get; set; }

        public abstract void Undo();

        public abstract void Redo();

        public abstract bool IsUndoable { get; }
    }
}
