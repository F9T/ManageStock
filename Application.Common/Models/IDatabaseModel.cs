using System;
using System.ComponentModel;

namespace Application.Common.Models
{
    public interface IDatabaseModel : ICloneable, INotifyPropertyChanged
    {
        int ZIndex { get; }

        object GetID();
    }
}
