using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MuVaViMo
{
    public class TransformingObservableReadOnlyList<TSource, TResult> : IObservableReadOnlyList<TResult>
    {
        private readonly IList<TResult> _backingList = new List<TResult>();

        public TransformingObservableReadOnlyList(ObservableCollection<TSource> source, Func<TSource, TResult> transform)
        {
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            ConnectToCollectionChanged(source, transform);
        }

        public TransformingObservableReadOnlyList(IObservableReadOnlyList<TSource> source, Func<TSource, TResult> transform)
        {
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            ConnectToCollectionChanged(source, transform);
        }

        private void ConnectToCollectionChanged(INotifyCollectionChanged source, Func<TSource, TResult> transform)
        {
            source.CollectionChanged += (sender, args) =>
            {
                switch(args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        int i = args.NewStartingIndex;
                        IList newItems = new List<TResult>();
                        foreach(TSource item in args.NewItems.Cast<TSource>())
                        { 
                            TResult newItem = transform(item);
                            newItems.Add(newItem);
                            _backingList.Insert(i++, newItem);
                        }
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                                                                                             newItems, args.NewStartingIndex));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        IList oldItems = new List<TResult>();
                        for(int l = 0; l < args.OldItems.Count; l++)
                        {
                            TResult oldItem = _backingList[args.OldStartingIndex];
                            oldItems.Add(oldItem);
                            _backingList.RemoveAt(args.OldStartingIndex);
                        }
                        CollectionChanged?.Invoke(this,
                                                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                                                                       oldItems, args.OldStartingIndex));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        IList replacedItems = new List<TResult>();
                        IList replacingItems = new List<TResult>();
                        for (int l = 0; l < args.OldItems.Count; l++)
                        {
                            TResult oldItem = _backingList[args.OldStartingIndex];
                            replacedItems.Add(oldItem);
                            _backingList.RemoveAt(args.OldStartingIndex);
                        }
                        int j = args.NewStartingIndex;
                        foreach (TSource item in args.NewItems.Cast<TSource>())
                        {
                            TResult newItem = transform(item);
                            replacingItems.Add(newItem);
                            _backingList.Insert(j++, transform(item));
                        }
                        CollectionChanged?.Invoke(this,
                                                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                                                                                       replacingItems, replacedItems,
                                                                                       args.OldStartingIndex));
                        if(args.OldItems.Count != args.NewItems.Count)
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        IList movedItems = new List<TResult>();
                        for (int l = 0; l < args.OldItems.Count; l++)
                        {
                            TResult movedItem = _backingList[args.OldStartingIndex];
                            movedItems.Add(movedItem);
                            _backingList.RemoveAt(args.OldStartingIndex);
                        }
                        int k = args.NewStartingIndex;
                        foreach(TResult movedItem in movedItems)
                        {
                            _backingList.Insert(k++, movedItem);
                        }
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move,
                                                                                             movedItems, args.NewStartingIndex,
                                                                                             args.OldStartingIndex));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _backingList.Clear();
                        CollectionChanged?.Invoke(this,
                                                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    default:
                        throw new Exception("Something unexpected happened with the source collection.");
                }
            };
        }

        #region Implementation of IEnumerable

        public IEnumerator<TResult> GetEnumerator()
        {
            return _backingList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out T>

        public int Count => _backingList.Count;

        #endregion

        #region Implementation of INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Implementation of IReadOnlyList<out TResult>

        public TResult this[int index] => _backingList[index];

        #endregion
    }
}
