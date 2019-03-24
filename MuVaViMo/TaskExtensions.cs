using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MuVaViMo
{
    public static class TaskExtensions
    {
        /// <summary>
        /// The enumeration which the task fetches is transformed to an IObservableReadOnlyList.
        /// The IObservableReadOnlyList however is returned immediately and fills itself as soon as the task is completed.
        /// The IObservableReadOnlyList has no further connection to the fetched enumeration. So if it is a list and gets additions afterward the IObservableReadOnlyList will not notice it.
        /// </summary>
        /// <typeparam name="TItem">Item type.</typeparam>
        /// <typeparam name="TEnumerable">Item type.</typeparam>
        /// <param name="task">Task which fetches the observable collection.</param>
        /// <returns></returns>
        public static IObservableReadOnlyList<TItem> ToObservableReadOnlyList<TEnumerable, TItem>(
            this Task<TEnumerable> task)
            where TEnumerable : IEnumerable<TItem>
        {
            var source = new ObservableCollection<TItem>();
            var returnedCollection = new WrappingObservableReadOnlyList<TItem>(source);

            task.ContinueWith(async t =>
            {
                if (t.IsFaulted)
                {
                    throw t.Exception;
                }

                foreach (var item in await t.ConfigureAwait(false))
                {
                    source.Add(item);
                }
            });

            return returnedCollection;
        }

        /// <summary>
        /// The observable collection which the task fetches is transformed to an IDeferredObservableReadOnlyList.
        /// The IDeferredObservableReadOnlyList however is returned immediately and connects to the observable connection as soon as the task is completed.
        /// The IDeferredObservableReadOnlyList stays connected to the observable collection and forwards all the notifications.
        /// </summary>
        /// <typeparam name="TItem">Item type.</typeparam>
        /// <param name="task">Task which fetches the observable collection.</param>
        /// <returns></returns>
        public static IDeferredObservableReadOnlyList<TItem> ToObservableReadOnlyList<TItem>(
            this Task<ObservableCollection<TItem>> task) => new DeferredWrappingObservableReadOnlyList<TItem>(task);
    }
}
