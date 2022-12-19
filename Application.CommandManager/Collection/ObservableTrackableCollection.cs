using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Application.CommandManager.Collection
{
    public class ObservableTrackableCollection<T> : ObservableCollection<T>
    {
        private CommandManager m_CommandManager;

        public ObservableTrackableCollection() => CollectionChanged += new NotifyCollectionChangedEventHandler(ObservableTrackableCollection_CollectionChanged);

        public ObservableTrackableCollection(IEnumerable<T> _Collection) : base(_Collection)
        {
            CollectionChanged += new NotifyCollectionChangedEventHandler(ObservableTrackableCollection_CollectionChanged);
        }

        public void InitializeTrackable(CommandManager _CommandManager)
        {
            m_CommandManager = _CommandManager;
            foreach (T obj in Items)
            {
                if ((object)obj is TrackableBase)
                {
                    ((object)obj as TrackableBase).InitializeTrackable(_CommandManager);
                }
            }
        }

        public CommandManager CommandManager
        {
            get => m_CommandManager;
            set => m_CommandManager = value;
        }

        private void ObservableTrackableCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (m_CommandManager == null)
            {
                return;
            }

            if (e.NewItems != null)
            {
                foreach (object newItem in e.NewItems)
                {
                    if (newItem is TrackableBase)
                    {
                        (newItem as TrackableBase).InitializeTrackable(CommandManager);
                    }
                }
            }
            m_CommandManager.AddCommand(new CollectionChangedCommand<T>(this, e.Action, e.NewItems, e.NewStartingIndex, e.OldItems, e.OldStartingIndex));
        }
    }
}
