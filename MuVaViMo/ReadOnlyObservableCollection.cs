using System;
using System.Collections.ObjectModel;

namespace MuVaViMo
{
    public static class ReadOnlyObservableCollectionExtensions
    {
        public static IObservableReadOnlyList<TResult> Transform<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> transform)
            => new TransformingObservableReadOnlyList<TSource, TResult>(source, transform);

        public static IObservableReadOnlyList<T> ToObservableReadOnlyList<T>(
            this ReadOnlyObservableCollection<T> source)
            => source.Transform(x => x);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ReadOnlyObservableCollection<T> first, ObservableCollection<T> second)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second.ToObservableReadOnlyList());

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ReadOnlyObservableCollection<T> first, IObservableReadOnlyList<T> second)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ReadOnlyObservableCollection<T> first, ReadOnlyObservableCollection<T> second)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second.ToObservableReadOnlyList());
    }
}
