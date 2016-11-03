using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MuVaViMo
{
    /// <summary>
    /// This interface is meant for read-only list, which synchronize with other observable collections/lists. 
    /// Each observed change is meant to be processed and forwarded (INotifyCollectionChanged, INotifyPropertyChanged).
    /// This lists should only forward changes of their source collections/lists and shouldn't be directly changeable (IReadOnlyList).
    /// Additionally, it is co-variant to the type T.
    /// </summary>
    /// <typeparam name="T">Co-variant type of the list's items.</typeparam>
    public interface IObservableReadOnlyList<out T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
    }
}