using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

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

    public static class ObservableReadOnlyListExtensions
    {
        public static ReadOnlyObservableCollection<T> ToReadOnlyObservableCollection<T>(
            this IObservableReadOnlyList<T> list)
        {
            var observableCollection = new ObservableCollection<T>(list);

            list.CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        int i = args.NewStartingIndex;
                        foreach (T item in args.NewItems.Cast<T>())
                        {
                            observableCollection.Insert(i++, item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        for (int l = 0; l < args.OldItems.Count; l++)
                        {
                            observableCollection.RemoveAt(args.OldStartingIndex);
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        for (int l = 0; l < args.OldItems.Count; l++)
                        {
                            observableCollection.RemoveAt(args.OldStartingIndex);
                        }
                        int j = args.NewStartingIndex;
                        foreach (T item in args.NewItems.Cast<T>())
                        {
                            observableCollection.Insert(j++, item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Move:
                        IList movedItems = new List<T>();
                        for (int l = 0; l < args.OldItems.Count; l++)
                        {
                            T movedItem = observableCollection[args.OldStartingIndex];
                            movedItems.Add(movedItem);
                            observableCollection.RemoveAt(args.OldStartingIndex);
                        }
                        int k = args.NewStartingIndex;
                        foreach (T movedItem in movedItems)
                        {
                            observableCollection.Insert(k++, movedItem);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        observableCollection.Clear();
                        break;
                    default:
                        throw new Exception("Something unexpected happened with the source collection.");
                }
            };

            return new ReadOnlyObservableCollection<T>(observableCollection);
        }
    }
}