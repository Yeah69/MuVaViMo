using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;

namespace MuVaViMo
{
    /// <summary>
    /// Concatenate two observable collections/lists and synchronizes with them.
    /// Note that there is no constructor for ObservableCollections directly, but there are comfortable static factory methods (Concatenate(...)) for that.
    /// </summary>
    /// <typeparam name="T">
    /// Item type of the forwarded items.
    /// Note that, because IObservableReadOnlyList is co-variant this type can be co-variant to both synchronized collection's/list's item types.
    /// </typeparam>
    public class ConcatenatingObservableReadOnlyList<T> : IObservableReadOnlyList<T>
    {
        /// <summary>
        /// Reference to first concatenated list.
        /// </summary>
        private readonly IReadOnlyList<T> _firstBackingList;
        /// <summary>
        /// Reference to second concatenated list.
        /// </summary>
        private readonly IReadOnlyList<T> _secondBackingList;

        /// <summary>
        /// Constructs a observable read only list, which concatenates two observable readonly lists.
        /// </summary>
        /// <param name="firstCollection"></param>
        /// <param name="secondCollection"></param>
        public ConcatenatingObservableReadOnlyList(IObservableReadOnlyList<T> firstCollection, IObservableReadOnlyList<T> secondCollection)
        {
            _firstBackingList = firstCollection;
            _secondBackingList = secondCollection;

            ConnectToCollectionChanged(firstCollection, secondCollection);
        }

        /// <summary>
        /// Constructs a observable read only list, which concatenates two observable readonly lists.
        /// </summary>
        /// <param name="firstCollection"></param>
        /// <param name="secondCollection"></param>
        /// <param name="notificationScheduler">On this scheduler the notifications are emitted.</param>
        public ConcatenatingObservableReadOnlyList(IObservableReadOnlyList<T> firstCollection, IObservableReadOnlyList<T> secondCollection, IScheduler notificationScheduler)
        {
            _firstBackingList = firstCollection;
            _secondBackingList = secondCollection;

            ConnectToCollectionChangedOnScheduler(firstCollection, secondCollection, notificationScheduler);
        }

        private void ConnectToCollectionChanged(INotifyCollectionChanged firstCollection, INotifyCollectionChanged secondCollection)
        {

            firstCollection.CollectionChanged += (_, args) => OnFirstCollectionChanged(args);

            secondCollection.CollectionChanged += (_, args) => OnSecondCollectionChanged(args);
        }

        private void ConnectToCollectionChangedOnScheduler(INotifyCollectionChanged firstCollection, INotifyCollectionChanged secondCollection, IScheduler notificationScheduler)
        {

            firstCollection.CollectionChanged += (_, args) => 
                notificationScheduler.Schedule(Unit.Default, (__, ___) => 
                    OnFirstCollectionChanged(args));

            secondCollection.CollectionChanged += (_, args) => 
                notificationScheduler.Schedule(Unit.Default, (__, ___) => 
                    OnSecondCollectionChanged(args));
        }

        private void OnFirstCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    CollectionChanged?.Invoke(this, args);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    CollectionChanged?.Invoke(this, args);
                    if (args.NewItems?.Count != args.OldItems?.Count) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    CollectionChanged?.Invoke(this, args);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                default:
                    throw new Exception("Something unexpected happened with the source collection.");
            }
        }

        private void OnSecondCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action, args.NewItems, args.NewStartingIndex + _firstBackingList.Count));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action, args.OldItems, args.OldStartingIndex + _firstBackingList.Count));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                case NotifyCollectionChangedAction.Move:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action, args.NewItems, args.NewStartingIndex + _firstBackingList.Count, args.OldStartingIndex + _firstBackingList.Count));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(args.Action, args.NewItems, args.OldItems, args.OldStartingIndex + _firstBackingList.Count));
                    if (args.NewItems?.Count != args.OldItems?.Count) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    CollectionChanged?.Invoke(this, args);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                default:
                    throw new Exception("Something unexpected happened with the source collection.");
            }
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

        public T this[int index]
        {
            get
            {
                if(index< 0 || index> Count) throw new ArgumentOutOfRangeException(nameof(index));
                return index<_firstBackingList.Count
                    ? _firstBackingList[index] 
                    : _secondBackingList[index - _firstBackingList.Count];
            }
        }

#endregion

/// <summary>
/// Constructs a concatenating read only list, which takes two ObservableCollections (which are wrapped beforehand).
/// </summary>
/// <typeparam name="TA">Item type of the first collection. Should inherit from T.</typeparam>
/// <typeparam name="TB">Item type of the second collection. Should inherit from T.</typeparam>
/// <param name="collectionA">First concatenated collection.</param>
/// <param name="collectionB">Second concatenated collection.</param>
/// <returns>A concatenating read only list, which takes two ObservableCollections.</returns>
public static ConcatenatingObservableReadOnlyList<T> Concatenate<TA, TB>(ObservableCollection<TA> collectionA,
                                                                                 ObservableCollection<TB> collectionB)
            where TA : class, T where TB : class, T
        {
            IObservableReadOnlyList<T> wrapA = new WrappingObservableReadOnlyList<TA>(collectionA);
            IObservableReadOnlyList<T> wrapB = new WrappingObservableReadOnlyList<TB>(collectionB);
            return new ConcatenatingObservableReadOnlyList<T>(wrapA, wrapB);
        }

        /// <summary>
        /// Constructs a concatenating read only list, which takes a ObservableCollection (first) and another observable read only list (second).
        /// </summary>
        /// <typeparam name="TA">Item type of the first collection. Should inherit from T.</typeparam>
        /// <param name="collectionA">First concatenated collection.</param>
        /// <param name="collectionB">Second concatenated collection.</param>
        /// <returns>A concatenating read only list, which takes a ObservableCollection and another observable read only list.</returns>
        public static ConcatenatingObservableReadOnlyList<T> Concatenate<TA>(ObservableCollection<TA> collectionA,
                                                                             IObservableReadOnlyList<T> collectionB)
            where TA : class, T
        {
            IObservableReadOnlyList<T> wrapA = new WrappingObservableReadOnlyList<TA>(collectionA);
            return new ConcatenatingObservableReadOnlyList<T>(wrapA, collectionB);
        }

        /// <summary>
        /// Constructs a concatenating read only list, which takes a ObservableCollection (second) and another observable read only list (first).
        /// </summary>
        /// <typeparam name="TB">Item type of the second collection. Should inherit from T.</typeparam>
        /// <param name="collectionA">First concatenated collection.</param>
        /// <param name="collectionB">Second concatenated collection.</param>
        /// <returns>A concatenating read only list, which takes a ObservableCollection and another observable read only list.</returns>
        public static ConcatenatingObservableReadOnlyList<T> Concatenate<TB>(IObservableReadOnlyList<T> collectionA,
                                                                             ObservableCollection<TB> collectionB)
            where TB : class, T
        {
            IObservableReadOnlyList<T> wrapB = new WrappingObservableReadOnlyList<TB>(collectionB);
            return new ConcatenatingObservableReadOnlyList<T>(collectionA, wrapB);
        }

        /// <summary>
        /// For the sake of completeness. Directly delegates construction to the constructor.
        /// </summary>
        /// <param name="collectionA">First concatenated collection.</param>
        /// <param name="collectionB">Second concatenated collection.</param>
        /// <returns>A concatenating read only list, which takes two observable read only list.</returns>
        public static ConcatenatingObservableReadOnlyList<T> Concatenate(IObservableReadOnlyList<T> collectionA,
                                                                             IObservableReadOnlyList<T> collectionB)
        {
            return new ConcatenatingObservableReadOnlyList<T>(collectionA, collectionB);
        }
    }
}