using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MuVaViMo
{
    public interface IObservableReadOnlyList<out T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}