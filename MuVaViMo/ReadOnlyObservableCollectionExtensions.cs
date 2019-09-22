using System;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;

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

        public static IObservableReadOnlyList<TResult> Transform<TSource, TResult>(
            this ReadOnlyObservableCollection<TSource> source, Func<TSource, TResult> transform, IScheduler notificationScheduler)
            => new TransformingObservableReadOnlyList<TSource, TResult>(source, transform, notificationScheduler);

        public static IObservableReadOnlyList<T> ToObservableReadOnlyList<T>(
            this ReadOnlyObservableCollection<T> source, IScheduler notificationScheduler)
            => source.Transform(x => x, notificationScheduler);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ReadOnlyObservableCollection<T> first, ObservableCollection<T> second, IScheduler notificationScheduler)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second.ToObservableReadOnlyList(), notificationScheduler);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ReadOnlyObservableCollection<T> first, IObservableReadOnlyList<T> second, IScheduler notificationScheduler)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second, notificationScheduler);

        public static IObservableReadOnlyList<T> Concatenate<T>(
            this ReadOnlyObservableCollection<T> first, ReadOnlyObservableCollection<T> second, IScheduler notificationScheduler)
            => new ConcatenatingObservableReadOnlyList<T>(first.ToObservableReadOnlyList(), second.ToObservableReadOnlyList(), notificationScheduler);
    }
}
