using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

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
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => ((INotifyPropertyChanged) _wrappedCollection).PropertyChanged += value;
            remove => ((INotifyPropertyChanged) _wrappedCollection).PropertyChanged -= value;
        }

        public void Move(int oldIndex, int newIndex)
        {
            _wrappedCollection.Move(oldIndex, newIndex);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _wrappedCollection.CollectionChanged += value;
            remove => _wrappedCollection.CollectionChanged -= value;
        }

        public void Clear()
        {
            _wrappedCollection.Clear();
        }
        
        public void Add(T item)
        {
            _wrappedCollection.Add(item);
        }

        public int Count => _wrappedCollection.Count;

        public bool Contains(T item)
        {
            return _wrappedCollection.Contains(item);
        }

        public void CopyTo(T[] array, int index)
        {
            _wrappedCollection.CopyTo(array, index);
        }

        public int IndexOf(T item)
        {
            return _wrappedCollection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _wrappedCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _wrappedCollection.RemoveAt(index);
        }

        public bool Remove(T item)
        {
            return _wrappedCollection.Remove(item);
        }

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

        public T this[int index] => _wrappedCollection[index];

        #endregion
    }
}
