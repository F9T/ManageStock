using Application.CommandManager.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Application.CommandManager
{
    public class CommandManager : INotifyPropertyChanged
    {
        private Stack<CommandGroup> m_UndoStack;
        private Stack<CommandGroup> m_RedoStack;
        private CommandGroup m_CurrentGroup;
        private bool m_IsSuspended = false;
        private CommandGroup m_LastSaveStackTop = null;

        public CommandManager()
        {
            m_UndoStack = new Stack<CommandGroup>();
            m_RedoStack = new Stack<CommandGroup>();
        }

        public void BeginGroup()
        {
            if (m_IsSuspended)
            {
                return;
            }

            m_CurrentGroup = new CommandGroup();
            m_UndoStack.Push(m_CurrentGroup);
        }

        public void EndGroup()
        {
            if (m_IsSuspended)
            {
                return;
            }

            m_CurrentGroup = null;
        }

        public void AddCommand(CommandBase _Command)
        {
            if (m_IsSuspended)
            {
                return;
            }

            if (m_CurrentGroup != null)
            {
                m_CurrentGroup.Commands.Add(_Command);
            }
            else
            {
                m_UndoStack.Push(new CommandGroup(_Command));
            }

            m_RedoStack.Clear();
            OnPropertyChanged("IsUndoEnable");
            OnPropertyChanged("IsRedoEnable");
            OnPropertyChanged("UndoStackCount");
            OnPropertyChanged("RedoStackCount");
            OnPropertyChanged("IsModified");
        }

        public void Undo()
        {
            if (!IsUndoEnable)
            {
                throw new Exception("Redo is not available !");
            }

            Suspend();
            CommandGroup _Commands = m_UndoStack.Pop();
            _Commands.Undo();
            m_RedoStack.Push(_Commands);
            OnPropertyChanged("IsUndoEnable");
            OnPropertyChanged("IsRedoEnable");
            OnPropertyChanged("UndoStackCount");
            OnPropertyChanged("RedoStackCount");
            OnPropertyChanged("IsModified");
            Resume();
            if (AfterUndo == null)
            {
                return;
            }

            AfterUndo(this, new CommandManagerEventArgs(_Commands) { CommandActionType = EnumCommandActionType.Undo });
        }

        public void Redo()
        {
            if (!IsRedoEnable)
            {
                throw new Exception("Redo is not available !");
            }

            Suspend();
            CommandGroup _Commands = m_RedoStack.Pop();
            _Commands.Redo();
            m_UndoStack.Push(_Commands);
            OnPropertyChanged("IsUndoEnable");
            OnPropertyChanged("IsRedoEnable");
            OnPropertyChanged("UndoStackCount");
            OnPropertyChanged("RedoStackCount");
            OnPropertyChanged("IsModified");
            Resume();
            if (AfterRedo == null)
            {
                return;
            }

            AfterRedo(this, new CommandManagerEventArgs(_Commands) { CommandActionType = EnumCommandActionType.Redo });
        }

        public bool IsSuspended => m_IsSuspended;

        public void Suspend() => m_IsSuspended = true;

        public void Resume() => m_IsSuspended = false;

        public bool IsUndoEnable => m_UndoStack.Count != 0 && m_UndoStack.First<CommandGroup>().IsUndoable;

        public bool IsRedoEnable => m_RedoStack.Count != 0 && m_RedoStack.First<CommandGroup>().IsUndoable;

        public int UndoStackCount => m_UndoStack.Count;

        public int RedoStackCount => m_RedoStack.Count;

        public void ResetModified()
        {
            m_LastSaveStackTop = m_UndoStack.FirstOrDefault<CommandGroup>();
            OnPropertyChanged("IsModified");
        }

        public bool IsModified => m_LastSaveStackTop != m_UndoStack.FirstOrDefault<CommandGroup>();

        public event EventHandler<CommandManagerEventArgs> AfterUndo;

        public event EventHandler<CommandManagerEventArgs> AfterRedo;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string _PropertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged == null)
            {
                return;
            }

            propertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
        }
    }
}
