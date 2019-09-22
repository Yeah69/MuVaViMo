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
    /// This class observes and synchronizes with a observable collection with items of type TSource, but forwards the items transformed to type TResult. 
    /// </summary>
    /// <typeparam name="TSource">Item's type of the observed collection.</typeparam>
    /// <typeparam name="TResult">Type of the forwarded items.</typeparam>
    public class TransformingObservableReadOnlyList<TSource, TResult> : IObservableReadOnlyList<TResult>
    {
        /// <summary>
        /// All transformed items go here. Synchronous to the source collection.
        /// </summary>
        private readonly IList<TResult> _backingList = new List<TResult>();

        /// <summary>
        /// Constructs a TransformingObservableReadOnlyList synchronizing with an ObservableCollection source.
        /// </summary>
        /// <param name="source">Collection to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        public TransformingObservableReadOnlyList(IEnumerable<TSource> source, Func<TSource, TResult> transform)
        {
            //The source collection may already have items. Thus, they are transformed and added to the backing list.
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            if (source is INotifyCollectionChanged collectionChanged)
                ConnectToCollectionChanged(collectionChanged, transform);
        }

        /// <summary>
        /// Constructs a TransformingObservableReadOnlyList synchronizing with an ObservableCollection source.
        /// </summary>
        /// <param name="source">Collection to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        public TransformingObservableReadOnlyList(ObservableCollection<TSource> source, Func<TSource, TResult> transform)
        {
            //The source collection may already have items. Thus, they are transformed and added to the backing list.
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            ConnectToCollectionChanged(source, transform);
        }

        /// <summary>
        /// Constructs a TransformingObservableReadOnlyList synchronizing with an IObservableReadOnlyList source.
        /// Intended to be used only by the constructors.
        /// </summary>
        /// <param name="source">List to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        public TransformingObservableReadOnlyList(IObservableReadOnlyList<TSource> source, Func<TSource, TResult> transform)
        {
            //The source collection may already have items. Thus, they are transformed and added to the backing list.
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            ConnectToCollectionChanged(source, transform);
        }

        /// <summary>
        /// Constructs a TransformingObservableReadOnlyList synchronizing with an ObservableCollection source.
        /// </summary>
        /// <param name="source">Collection to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        /// <param name="notificationScheduler">On this scheduler the notifications are emitted.</param>
        public TransformingObservableReadOnlyList(IEnumerable<TSource> source, Func<TSource, TResult> transform, IScheduler notificationScheduler)
        {
            //The source collection may already have items. Thus, they are transformed and added to the backing list.
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            if (source is INotifyCollectionChanged collectionChanged)
                ConnectToCollectionChangedOnScheduler(collectionChanged, transform, notificationScheduler);
        }

        /// <summary>
        /// Constructs a TransformingObservableReadOnlyList synchronizing with an ObservableCollection source.
        /// </summary>
        /// <param name="source">Collection to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        /// <param name="notificationScheduler">On this scheduler the notifications are emitted.</param>
        public TransformingObservableReadOnlyList(ObservableCollection<TSource> source, Func<TSource, TResult> transform, IScheduler notificationScheduler)
        {
            //The source collection may already have items. Thus, they are transformed and added to the backing list.
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            ConnectToCollectionChangedOnScheduler(source, transform, notificationScheduler);
        }

        /// <summary>
        /// Constructs a TransformingObservableReadOnlyList synchronizing with an IObservableReadOnlyList source.
        /// Intended to be used only by the constructors.
        /// </summary>
        /// <param name="source">List to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        /// <param name="notificationScheduler">On this scheduler the notifications are emitted.</param>
        public TransformingObservableReadOnlyList(IObservableReadOnlyList<TSource> source, Func<TSource, TResult> transform, IScheduler notificationScheduler)
        {
            //The source collection may already have items. Thus, they are transformed and added to the backing list.
            foreach (TSource from in source)
            {
                _backingList.Add(transform(from));
            }

            ConnectToCollectionChangedOnScheduler(source, transform, notificationScheduler);
        }

        /// <summary>
        /// Connects the source collection to this collection and forwards the events according to the transforming logic.
        /// </summary>
        /// <param name="source">List to synchronize with.</param>
        /// <param name="transform">Logic to transform a TSource object to a TResult object.</param>
        private void ConnectToCollectionChanged(INotifyCollectionChanged source, Func<TSource, TResult> transform)
        {

            source.CollectionChanged += (_, args) => OnSourceOnCollectionChanged(args, transform);
        }

        private void ConnectToCollectionChangedOnScheduler(INotifyCollectionChanged source, Func<TSource, TResult> transform, IScheduler notificationScheduler)
        {
            source.CollectionChanged += (_, args) => 
                notificationScheduler.Schedule(Unit.Default, (__, ___) => 
                    OnSourceOnCollectionChanged(args, transform));
        }

        private void OnSourceOnCollectionChanged(
            NotifyCollectionChangedEventArgs args, 
            Func<TSource, TResult> transform)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int i = args.NewStartingIndex;
                    IList newItems = new List<TResult>();
                    foreach (TSource item in args.NewItems.Cast<TSource>())
                    {
                        TResult newItem = transform(item);
                        newItems.Add(newItem);
                        _backingList.Insert(i++, newItem);
                    }

                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, args.NewStartingIndex));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    IList oldItems = new List<TResult>();
                    for (int l = 0; l < args.OldItems.Count; l++)
                    {
                        TResult oldItem = _backingList[args.OldStartingIndex];
                        oldItems.Add(oldItem);
                        _backingList.RemoveAt(args.OldStartingIndex);
                    }

                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems, args.OldStartingIndex));
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

                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, replacingItems, replacedItems, args.OldStartingIndex));
                    if (args.OldItems.Count != args.NewItems.Count) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
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
                    foreach (TResult movedItem in movedItems)
                    {
                        _backingList.Insert(k++, movedItem);
                    }

                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItems, args.NewStartingIndex, args.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _backingList.Clear();
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
                    break;
                default:
                    throw new Exception("Something unexpected happened with the source collection.");
            }
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

        public TResult this[int index]
        {
            get
            {
                if(index< 0 || index> Count) throw new ArgumentOutOfRangeException(nameof(index));
                return _backingList[index];
            }
        }

        #endregion
    }
}
