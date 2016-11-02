using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MuVaViMo
{
    public class ConcatenatingObservableReadOnlyList<T> : IObservableReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _firstBackingList;
        private readonly IReadOnlyList<T> _secondBackingList;

        public ConcatenatingObservableReadOnlyList(IObservableReadOnlyList<T> firstCollection, IObservableReadOnlyList<T> secondCollection)
        {
            _firstBackingList = firstCollection;
            _secondBackingList = secondCollection;

            ConnectToCollectionChanged(firstCollection, secondCollection);
        }

        private void ConnectToCollectionChanged(INotifyCollectionChanged firstCollection, INotifyCollectionChanged secondCollection)
        {
            firstCollection.CollectionChanged += (sender, args) =>
            {
                switch(args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Remove:
                        CollectionChanged?.Invoke(this, args);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Replace:
                        CollectionChanged?.Invoke(this, args);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        CollectionChanged?.Invoke(this,
                                                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                                                                       _firstBackingList, 0));
                        break;
                    default:
                        throw new Exception("Something unexpected happened with the source collection.");
                }
            };

            secondCollection.CollectionChanged += (sender, args) =>
            {
                switch(args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action,
                                                                                             args.NewItems,
                                                                                             args.NewStartingIndex +
                                                                                             _firstBackingList.Count));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action,
                                                                                             args.OldItems,
                                                                                             args.OldStartingIndex +
                                                                                             _firstBackingList.Count));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                        break;
                    case NotifyCollectionChangedAction.Move:
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action,
                                                                                             args.NewItems,
                                                                                             args.NewStartingIndex +
                                                                                             _firstBackingList.Count,
                                                                                             args.OldStartingIndex +
                                                                                             _firstBackingList.Count));
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action,
                                                                                             args.NewItems, args.OldItems,
                                                                                             args.OldStartingIndex +
                                                                                             _firstBackingList.Count));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        CollectionChanged?.Invoke(this,
                                                  new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                                                                       _secondBackingList,
                                                                                       _firstBackingList.Count));
                        break;
                    default:
                        throw new Exception("Something unexpected happened with the source collection.");
                }
            };
        }

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<out T>

        public IEnumerator<T> GetEnumerator()
        {
            return _firstBackingList.Concat(_secondBackingList).GetEnumerator();
        }

        #endregion

        #region Implementation of INotifyCollectionChanged

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Implementation of IReadOnlyCollection<out T>

        public int Count => _firstBackingList.Count + _secondBackingList.Count;

        #endregion

        #region Implementation of IReadOnlyList<out T>

        public T this[int index] => index < _firstBackingList.Count
                                        ? _firstBackingList[index] 
                                        : _secondBackingList[index - _firstBackingList.Count];

        #endregion
    }
}