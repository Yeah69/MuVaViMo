using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MuVaViMo
{
    public class TransformingObservableCollectionWrapper<TSource, TResult> : IReadOnlyList<TResult>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly List<TResult> _backingCollection = new List<TResult>();

        public TransformingObservableCollectionWrapper(ObservableCollection<TSource> source, Func<TSource, TResult> transform)
        {
            foreach(TSource from in source)
            {
                _backingCollection.Add(transform(from));
            }

            source.CollectionChanged += (sender, args) =>
            {
                //Keep in mind the source is an ObservableCollection.
                //Thus, each CollectionChanged event has at most one item in NewItems and/or OldItems.
                //http://www.codeproject.com/Articles/1004644/ObservableCollection-Simply-Explained
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        TResult newItem = transform((TSource) args.NewItems[0]);
                        _backingCollection.Insert(args.NewStartingIndex, newItem);
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 
                            newItem, args.NewStartingIndex));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        TResult oldItem = _backingCollection[args.OldStartingIndex];
                        _backingCollection.RemoveAt(args.OldStartingIndex);
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, 
                            oldItem, args.OldStartingIndex));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        TResult replacingItem = transform((TSource) args.NewItems[0]);
                        TResult replacedItem = _backingCollection[args.OldStartingIndex];
                        _backingCollection[args.OldStartingIndex] = replacingItem;
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, 
                            replacingItem, replacedItem, args.OldStartingIndex));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        TResult movedItem = _backingCollection[args.OldStartingIndex];
                        _backingCollection.RemoveAt(args.OldStartingIndex);
                        _backingCollection.Insert(args.NewStartingIndex, movedItem);
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, 
                            movedItem, args.NewStartingIndex, args.OldStartingIndex));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _backingCollection.Clear();
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
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
            return _backingCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IReadOnlyCollection<out T>

        public int Count => _backingCollection.Count;

        #endregion

        #region Implementation of INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Implementation of IReadOnlyList<out TResult>

        public TResult this[int index] => _backingCollection[index];

        #endregion
    }
}
