using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Application.CommandManager;
using Application.Common.Models.Items;

namespace Application.Common.Models.Groups
{
    public abstract class GroupBase<T> : TrackableBase, IDatabaseModel where T : ItemBase
    {
        public Guid ID { get; set; }

        public T Item { get; set; }

        public object GetID()
        {
            return ID;
        }

        public abstract int ZIndex { get; }

        public abstract object Clone();

        public abstract void CopyTo(GroupBase<T> _Group);
    }
}
