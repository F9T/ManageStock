using System;
using System.Windows.Input;

namespace Application.Common.Commands
{
    public class RelayCommand : ICommand
    {
        private Action<object> m_Execute;

        private Predicate<object> m_CanExecute;

        private event EventHandler CanExecuteChangedInternal;

        public RelayCommand(Action<object> _Execute)
            : this(_Execute, DefaultCanExecute)
        {
        }

        public RelayCommand(Action<object> _Execute, Predicate<object> _CanExecute)
        {
            if (_Execute == null)
            {
                throw new ArgumentNullException("_Execute");
            }

            if (_CanExecute == null)
            {
                throw new ArgumentNullException("_CanExecute");
            }

            this.m_Execute = _Execute;
            this.m_CanExecute = _CanExecute;
        }

        public Predicate<object> CanExecutePredicate
        {
            get
            {
                return m_CanExecute;
            }
            set
            {
                m_CanExecute = value;
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                System.Windows.Input.CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                System.Windows.Input.CommandManager.RequerySuggested -= value;                
                this.CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return this.m_CanExecute != null && this.m_CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.m_Execute(parameter);
        }

        public void OnCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChangedInternal;
            if (handler != null)
            {
                //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()
        {
            this.m_CanExecute = _ => false;
            this.m_Execute = _ => { return; };
        }

        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}
