using System;

namespace Application.Common.Navigator
{
    public class NavigatorItemChangedEventArgs<T> : EventArgs
    {
        public NavigatorItemChangedEventArgs(T _Item)
        {
            Item = _Item;
        }

        public T Item { get; private set; }
    }
}
