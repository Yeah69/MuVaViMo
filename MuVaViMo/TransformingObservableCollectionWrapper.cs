using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MuVaViMo
{
    public class TransformingObservableCollectionWrapper<TSource, TResult> : IReadOnlyCollection<TResult>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly Collection<TResult> _backingCollection = new Collection<TResult>();

        public TransformingObservableCollectionWrapper(ObservableCollection<TSource> source, Func<TSource, TResult> transform)
        {
            foreach(TSource from in source)
            {
                _backingCollection.Add(transform(from));
            }

            source.CollectionChanged += (sender, args) =>
            {
                switch(args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        TResult newItem = transform((TSource) args.NewItems[0]);
                        _backingCollection.Insert(args.NewStartingIndex, newItem);
                        RaiseCollectionChanged(NotifyCollectionChangedAction.Add, newItem, args.NewStartingIndex);
                        RaisePropertyChanged(nameof(_backingCollection.Count));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        TResult oldItem = _backingCollection[args.OldStartingIndex];
                        _backingCollection.RemoveAt(args.OldStartingIndex);
                        RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, oldItem, args.OldStartingIndex);
                        RaisePropertyChanged(nameof(_backingCollection.Count));
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _backingCollection.Clear();
                        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                        RaisePropertyChanged(nameof(_backingCollection.Count));
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

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, object item, int index) =>
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, item, index));

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}
