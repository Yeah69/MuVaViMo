using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace MuVaViMo
{
    /// <summary>
    /// Wraps an ObservableCollection which is fetched by a task and adds the IObservableReadOnlyList interface to it.
    /// </summary>
    /// <typeparam name="T">Item type of the wrapped collection.</typeparam>
    public class DeferredWrappingObservableReadOnlyList<T> : IDeferredObservableReadOnlyList<T>
    {
        /// <summary>
        /// Wrapped collection.
        /// </summary>
        private ObservableCollection<T> _wrappedCollection;

        private readonly Task _initialization;

        /// <summary>
        /// Constructs a wrapping read only list, which synchronizes with the wrapped collection.
        /// </summary>
        /// <param name="wrappedCollectionTask">Task which fetches the wrapped collection.</param>
        public DeferredWrappingObservableReadOnlyList(Task<ObservableCollection<T>> wrappedCollectionTask)
        {
            _initialization = wrappedCollectionTask.ContinueWith(async t =>
            {
                try
                {
                    var observableCollection = await t.ConfigureAwait(false);
                    _wrappedCollection = observableCollection;
                    ConnectToCollectionChanged(observableCollection, observableCollection);
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        /// <summary>
        /// Constructs a wrapping read only list, which synchronizes with the wrapped collection.
        /// </summary>
        /// <param name="wrappedCollectionTask">Task which fetches the wrapped collection.</param>
        /// <param name="notificationScheduler">On this scheduler the notifications are emitted.</param>
        public DeferredWrappingObservableReadOnlyList(Task<ObservableCollection<T>> wrappedCollectionTask, IScheduler notificationScheduler)
        {
            _initialization = wrappedCollectionTask.ContinueWith(async t =>
            {
                try
                {
                    var observableCollection = await t.ConfigureAwait(false);
                    _wrappedCollection = observableCollection;
                    ConnectToCollectionChangedOnScheduler(observableCollection, observableCollection, notificationScheduler);
                }
                catch (OperationCanceledException)
                {
                }
            });
        }

        private void ConnectToCollectionChanged(INotifyCollectionChanged collection, INotifyPropertyChanged property)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _wrappedCollection, 0));
            collection.CollectionChanged += (sender, args) => CollectionChanged?.Invoke(this, args);
            property.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
        }

        private void ConnectToCollectionChangedOnScheduler(INotifyCollectionChanged collection, INotifyPropertyChanged property, IScheduler notificationScheduler)
        {
            notificationScheduler.Schedule(Unit.Default, (_, __) =>
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _wrappedCollection, 0)));
            collection.CollectionChanged += (sender, args) => 
                notificationScheduler.Schedule(Unit.Default, (_, __) => 
                    CollectionChanged?.Invoke(this, args));
            property.PropertyChanged += (sender, args) => 
                notificationScheduler.Schedule(Unit.Default, (_, __) => 
                    PropertyChanged?.Invoke(this, args));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count => _wrappedCollection?.Count ?? 0;
        
        #region Implementation of IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return _wrappedCollection?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();
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
                if(_wrappedCollection is null) throw new Exception("Not initialized yet");
                if(index < 0 || index > Count) throw new ArgumentOutOfRangeException(nameof(index));
                return _wrappedCollection[index];
            }
        }

        #endregion

        #region Implementation of IDeferredObservableReadOnlyList<T>

        public Task<IDeferredObservableReadOnlyList<T>> InitializedCollectionAsync => 
            _initialization.ContinueWith(_ => (IDeferredObservableReadOnlyList<T>) this);

        #endregion
    }
}
