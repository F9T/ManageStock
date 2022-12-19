using Application.CommandManager;

namespace Application.Common.Models.Items
{
    public abstract class ItemBase : TrackableBase, IDatabaseModel
    {
        public int ID { get; set; }

        public bool IsModified { get; set; }
        public object GetID()
        {
            return ID;
        }
        public abstract int ZIndex { get; }

        public abstract void Default();

        public abstract object Clone();
    }
}
