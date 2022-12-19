using Application.CommandManager.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Application.CommandManager.Collection
{
    public class CollectionChangedCommand<T> : CommandBase
    {
        private IList<T> m_Source;
        private NotifyCollectionChangedAction m_Action;
        private IList m_NewItems;
        private int m_NewStartingIndex;
        private IList m_OldItems;
        private int m_OldStartingIndex;

        public CollectionChangedCommand(IList<T> _Source, NotifyCollectionChangedAction _Action, IList _NewItems, int _NewStartingIndex, IList _OldItems, int _OldStartingIndex)
        {
            m_Source = _Source;
            m_Action = _Action;
            m_NewItems = _NewItems;
            m_NewStartingIndex = _NewStartingIndex;
            m_OldItems = _OldItems;
            m_OldStartingIndex = _OldStartingIndex;
            if (_Action == NotifyCollectionChangedAction.Add || _Action == NotifyCollectionChangedAction.Replace)
            {
                Context = _NewItems[0];
            }
            else if (_Action == NotifyCollectionChangedAction.Remove)
            {
                Context = _OldItems[0];
            }
            else
            {
                Context = typeof(T);
            }
        }

        public NotifyCollectionChangedAction Action => m_Action;

        public override void Undo()
        {
            switch (m_Action)
            {
                case NotifyCollectionChangedAction.Add:
                    IEnumerator enumerator1 = m_NewItems.GetEnumerator();
                    try
                    {
                        while (enumerator1.MoveNext())
                        {
                            m_Source.Remove((T)enumerator1.Current);
                        }

                        break;
                    }
                    finally
                    {
                        if (enumerator1 is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                case NotifyCollectionChangedAction.Remove:
                    IEnumerator enumerator2 = m_OldItems.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            m_Source.Insert(m_OldStartingIndex, (T)enumerator2.Current);
                        }

                        break;
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                case NotifyCollectionChangedAction.Replace:
                    object oldItem = m_OldItems[0];
                    m_Source[m_Source.IndexOf((T)m_NewItems[0])] = (T)oldItem;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    IEnumerator enumerator3 = m_OldItems.GetEnumerator();
                    try
                    {
                        while (enumerator3.MoveNext())
                        {
                            m_Source.Add((T)enumerator3.Current);
                        }

                        break;
                    }
                    finally
                    {
                        if (enumerator3 is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Redo()
        {
            switch (m_Action)
            {
                case NotifyCollectionChangedAction.Add:
                    IEnumerator enumerator1 = m_NewItems.GetEnumerator();
                    try
                    {
                        while (enumerator1.MoveNext())
                        {
                            m_Source.Insert(m_NewStartingIndex, (T)enumerator1.Current);
                        }

                        break;
                    }
                    finally
                    {
                        if (enumerator1 is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                case NotifyCollectionChangedAction.Remove:
                    IEnumerator enumerator2 = m_OldItems.GetEnumerator();
                    try
                    {
                        while (enumerator2.MoveNext())
                        {
                            m_Source.Remove((T)enumerator2.Current);
                        }

                        break;
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                case NotifyCollectionChangedAction.Replace:
                    object oldItem = m_OldItems[0];
                    object newItem = m_NewItems[0];
                    m_Source[m_Source.IndexOf((T)oldItem)] = (T)newItem;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    m_Source.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool IsUndoable => true;
    }
}
