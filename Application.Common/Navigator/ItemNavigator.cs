using Application.Common.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Application.Common.Navigator
{
    public class ItemNavigator<T> : INotifyPropertyChanged
    {
        private List<T> m_List;
        private int m_CurrentIndex;
        private T m_Current;
        private bool m_IsSuspend;

        public event EventHandler<NavigatorItemChangedEventArgs<T>> ItemOnChanged;

        public ItemNavigator()
        {
            m_IsSuspend = false;
            m_List = new List<T>();
            m_CurrentIndex = -1;

            PreviousCommand = new RelayCommand(_ => Previous(), _ => HasPrevious);
            NextCommand = new RelayCommand(_ => Next(), _ => HasNext);
        }

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        private bool HasPrevious => m_CurrentIndex > 0;

        private bool HasNext => m_CurrentIndex <= m_List.Count - 2;

        public void Suspend()
        {
            m_IsSuspend = true;
        }

        public void Resume()
        {
            m_IsSuspend = false;
        }

        public void Add(T _Item)
        {
            if (_Item == null || m_IsSuspend)
            {
                return;
            }

            if(m_List.Count > 0 && m_List.Last().Equals(_Item))
            {
                return;
            }

            bool hasNext = HasNext;

            if (hasNext)
            {
                RemoveUntilTheEnd();
            }

            m_List.Add(_Item);
            Move(1);
        }

        public void Reset()
        {
            m_List.Clear();
            m_CurrentIndex = -1;

            NotifyChanged();
        }

        private void Previous()
        {
            if (HasPrevious)
            {
                Move(-1);
            }
        }

        private void Next()
        {
            if (HasNext)
            {
                Move(1);
            }
        }

        private void RemoveUntilTheEnd()
        {
            for(int i = m_CurrentIndex+1; i < m_List.Count;)
            {
                m_List.RemoveAt(i);
            }
        }

        private void Move(int _IncrValue)
        {
            m_CurrentIndex += _IncrValue;

            m_Current = m_List[m_CurrentIndex];

            NotifyChanged();

            Suspend();
            OnItemOnChanged(m_Current);
            Resume();
        }

        private void NotifyChanged()
        {
            OnPropertyChanged(nameof(HasPrevious));
            OnPropertyChanged(nameof(HasNext));
        }
        protected virtual void OnItemOnChanged(T _Item)
        {
            ItemOnChanged?.Invoke(this, new NavigatorItemChangedEventArgs<T>(_Item));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
