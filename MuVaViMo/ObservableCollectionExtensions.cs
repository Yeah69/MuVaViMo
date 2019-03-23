using System;
using System.Collections.ObjectModel;

namespace MuVaViMo
{
    public static class ObservableCollectionExtensions
    {
        public static IObservableReadOnlyList<TResult> Transform<TSource, TResult>(
            this ObservableCollection<TSource> source, Func<TSource, TResult> transform)
            => new TransformingObservableReadOnlyList<TSource, TResult>(source, transform);

        public static IObservableReadOnlyList<T> ToObservableReadOnlyList<T>(
            this ObservableCollection<T> source)
            => new WrappingObservableReadOnlyList<T>(source);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ObservableCollection<T> first, ObservableCollection<T> second)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second.ToObservableReadOnlyList());

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ObservableCollection<T> first, IObservableReadOnlyList<T> second)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ObservableCollection<T> first, ReadOnlyObservableCollection<T> second)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second.ToObservableReadOnlyList());
    }
}
