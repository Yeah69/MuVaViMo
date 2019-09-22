using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;

namespace MuVaViMo
{
    /// <summary>
    /// Wraps an ObservableCollection and adds the IObservableReadOnlyList interface to it.
    /// </summary>
    /// <typeparam name="T">Item type of the wrapped collection.</typeparam>
    public class WrappingObservableReadOnlyList<T> : IObservableReadOnlyList<T>
    {
        /// <summary>
        /// Wrapped collection.
        /// </summary>
        private readonly ObservableCollection<T> _wrappedCollection;

        /// <summary>
        /// Constructs a wrapping read only list, which synchronizes with the wrapped collection.
        /// </summary>
        /// <param name="wrappedCollection">Wrapped collection.</param>
        public WrappingObservableReadOnlyList(ObservableCollection<T> wrappedCollection)
        {
            _wrappedCollection = wrappedCollection;
            ConnectToCollectionChanged(wrappedCollection, wrappedCollection);
        }

        /// <summary>
        /// Constructs a wrapping read only list, which synchronizes with the wrapped collection.
        /// </summary>
        /// <param name="wrappedCollection">Wrapped collection.</param>
        /// <param name="notificationScheduler">On this scheduler the notifications are emitted.</param>
        public WrappingObservableReadOnlyList(ObservableCollection<T> wrappedCollection, IScheduler notificationScheduler)
        {
            _wrappedCollection = wrappedCollection;
            ConnectToCollectionChangedOnScheduler(wrappedCollection, wrappedCollection, notificationScheduler);
        }

        /// <summary>
        /// Connects to the given collection and forwards the notification events.
        /// </summary>
        /// <param name="collection">The collection to connect to.</param>
        /// <param name="property">The collection to connect to.</param>
        private void ConnectToCollectionChanged(INotifyCollectionChanged collection, INotifyPropertyChanged property)
        {
            collection.CollectionChanged += (_, args) => CollectionChanged?.Invoke(this, args);
            property.PropertyChanged += (_, args) => PropertyChanged?.Invoke(this, args);
        }

        private void ConnectToCollectionChangedOnScheduler(INotifyCollectionChanged collection, INotifyPropertyChanged property, IScheduler notificationScheduler)
        {
            collection.CollectionChanged += (_, args) => 
                notificationScheduler.Schedule(Unit.Default, (__, ___) => 
                    CollectionChanged?.Invoke(this, args));
            property.PropertyChanged += (_, args) =>
                notificationScheduler.Schedule(Unit.Default, (__, ___) =>
                    PropertyChanged?.Invoke(this, args));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _wrappedCollection.Count;

        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _wrappedCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IReadOnlyList<out T>

        public T this[int index]
        {
            get
            {
                if (index < 0 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));
                return _wrappedCollection[index];
            }
        }

        #endregion
    }
}
