using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace MuVaViMo.Tests
{
    public class DeferredWrappingObservableReadOnlyListTests
    {
        class A { }

        ObservableCollection<A> FilledSourceCollection => new ObservableCollection<A> { new A(), new A(), new A(), new A(), new A(), new A() };

        [Fact]
        public async Task Constr_EmptySourceCollectionAndAwaitingInitialization_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = new ObservableCollection<A>();
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task ConstrLongLastingTask_EmptySourceCollectionAndAwaitingInitialization_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = new ObservableCollection<A>();
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);


            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void ConstrLongLastingTask_EmptySourceCollectionAndNotAwaitingInitialization_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = new ObservableCollection<A>();
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act


            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Constr_FilledCollectionAndAwaitingInitialization_ElementsCanBeIterated()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.FromResult(filledCollection));

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task ConstrLongLastingTask_FilledCollectionAndAwaitingInitialization_ElementsCanBeIterated()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void ConstrLongLastingTask_FilledCollectionAndNotAwaitingInitialization_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Addition_FilledCollectionAndAwaitingInitializationBeforeAddition_AddedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            A addedA = new A();

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Add(addedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task Addition_FilledCollectionAndAwaitingInitializationAfterAddition_AddedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            A addedA = new A();

            //Act
            filledCollection.Add(addedA);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task AdditionLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeAddition_AddedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A addedA = new A();

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Add(addedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task AdditionLongLastingTask_FilledCollectionAndAwaitingInitializationAfterAddition_AddedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A addedA = new A();

            //Act
            filledCollection.Add(addedA);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void AdditionLongLastingTask_FilledCollectionAndNotAwaitingInitializationBeforeAddition_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A addedA = new A();

            //Act
            filledCollection.Add(addedA);

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Insertion_FilledCollectionAndAwaitingInitializationBeforeInsertion_InsertedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            A insertedA = new A();
            int insertIndex = 1;

            //Act
            filledCollection.Insert(insertIndex, insertedA);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task Insertion_FilledCollectionAndAwaitingInitializationAfterInsertion_InsertedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            A insertedA = new A();
            int insertIndex = 1;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Insert(insertIndex, insertedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task InsertionLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeInsertion_InsertedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A insertedA = new A();
            int insertIndex = 1;

            //Act
            filledCollection.Insert(insertIndex, insertedA);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task InsertionLongLastingTask_FilledCollectionAndAwaitingInitializationAfterInsertion_InsertedElementIncluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A insertedA = new A();
            int insertIndex = 1;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Insert(insertIndex, insertedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void InsertionLongLastingTask_FilledCollectionAndNotAwaitingInitializationBeforeInsertion_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A insertedA = new A();
            int insertIndex = 1;

            //Act
            filledCollection.Insert(insertIndex, insertedA);

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task RemoveAt_FilledCollectionAndAwaitingInitializationBeforeRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            int removedIndex = 1;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.RemoveAt(removedIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task RemoveAt_FilledCollectionAndAwaitingInitializationAfterRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            int removedIndex = 1;

            //Act
            filledCollection.RemoveAt(removedIndex);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task RemoveAtLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int removedIndex = 1;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.RemoveAt(removedIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task RemoveAtLongLastingTask_FilledCollectionAndAwaitingInitializationAfterRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int removedIndex = 1;

            //Act
            filledCollection.RemoveAt(removedIndex);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void RemoveAtLongLastingTask_FilledCollectionAndNotAwaitingInitializationBeforeRemoval_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int removedIndex = 1;

            //Act
            filledCollection.RemoveAt(removedIndex);

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Remove_FilledCollectionAndAwaitingInitializationBeforeRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Remove(removedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task Remove_FilledCollectionAndAwaitingInitializationAfterRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];

            //Act
            filledCollection.Remove(removedA);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task RemoveLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Remove(removedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task RemoveLongLastingTask_FilledCollectionAndAwaitingInitializationAfterRemoval_RemovedElementExcluded()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];

            //Act
            filledCollection.Remove(removedA);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void RemoveLongLastingTask_FilledCollectionAndNotAwaitingInitializationBeforeRemoval_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];

            //Act
            filledCollection.Remove(removedA);

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Clear_FilledCollectionAndAwaitingInitializationBeforeClearance_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Clear();

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task Clear_FilledCollectionAndAwaitingInitializationAfterClearance_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));

            //Act
            filledCollection.Clear();
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task ClearLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeClearance_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Clear();

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task ClearLongLastingTask_FilledCollectionAndAwaitingInitializationAfterClearance_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act
            filledCollection.Clear();
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void ClearLongLastingTask_FilledCollectionAndNotAwaitingInitializationBeforeClearance_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));

            //Act
            filledCollection.Clear();

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Replace_FilledCollectionAndAwaitingInitializationBeforeReplacement_IncludesReplacedElement()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            A replacingA = new A();
            int replaceIndex = 1;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection[replaceIndex] = replacingA;

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task Replace_FilledCollectionAndAwaitingInitializationAfterReplacement_IncludesReplacedElement()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            A replacingA = new A();
            int replaceIndex = 1;

            //Act
            filledCollection[replaceIndex] = replacingA;
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task ReplaceLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeReplacement_IncludesReplacedElement()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A replacingA = new A();
            int replaceIndex = 1;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection[replaceIndex] = replacingA;

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task ReplaceLongLastingTask_FilledCollectionAndAwaitingInitializationAfterReplacement_IncludesReplacedElement()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A replacingA = new A();
            int replaceIndex = 1;

            //Act
            filledCollection[replaceIndex] = replacingA;
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void ReplaceLongLastingTask_FilledCollectionAndNOtAwaitingInitializationBeforeReplacement_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            A replacingA = new A();
            int replaceIndex = 1;

            //Act
            filledCollection[replaceIndex] = replacingA;

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task Move_FilledCollectionAndAwaitingInitializationBeforeMovement_MovedElementOnRightPosition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            int moveFromIndex = 1;
            int moveToIndex = 3;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Move(moveFromIndex, moveToIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task Move_FilledCollectionAndAwaitingInitializationAfterMovement_MovedElementOnRightPosition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(Task.FromResult(filledCollection));
            int moveFromIndex = 1;
            int moveToIndex = 3;

            //Act
            filledCollection.Move(moveFromIndex, moveToIndex);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task MoveLongLastingTask_FilledCollectionAndAwaitingInitializationBeforeMovement_MovedElementOnRightPosition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int moveFromIndex = 1;
            int moveToIndex = 3;

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);
            filledCollection.Move(moveFromIndex, moveToIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public async Task MoveLongLastingTask_FilledCollectionAndAwaitingInitializationAfterMovement_MovedElementOnRightPosition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int moveFromIndex = 1;
            int moveToIndex = 3;

            //Act
            filledCollection.Move(moveFromIndex, moveToIndex);
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void MoveLongLastingTask_FilledCollectionAndNotAwaitingInitializationBeforeMovement_NoElements()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledCollection;
                }));
            int moveFromIndex = 1;
            int moveToIndex = 3;

            //Act
            filledCollection.Move(moveFromIndex, moveToIndex);

            //Assert
            UninitializedCheck(readOnlyList);
        }

        [Fact]
        public async Task DeferredLinkedToTransforming_AwaitInitialization_TransformingHasAllInitializedElements()
        {
            //Arrange
            var filledSourceCollection = FilledSourceCollection;
            DeferredWrappingObservableReadOnlyList<A> readOnlyList = new DeferredWrappingObservableReadOnlyList<A>(
                Task.Run(async () =>
                {
                    await Task.Delay(100).ConfigureAwait(false);
                    return filledSourceCollection;
                }));
            TransformingObservableReadOnlyList<A, A> proxy = new TransformingObservableReadOnlyList<A, A>(
                readOnlyList, a => a);

            //Act
            await readOnlyList.InitializedCollectionAsync.ConfigureAwait(false);

            //Assert
            StandardCheck(filledSourceCollection, proxy);
        }

        private static void StandardCheck(ObservableCollection<A> filledCollection,
            IObservableReadOnlyList<A> readOnlyList)
        {
            Assert.Equal(filledCollection.Count, readOnlyList.Count);
            int index = 0;
            foreach (A a in readOnlyList)
            {
                Assert.Same(a, filledCollection[index++]);
            }
        }

        private static void UninitializedCheck(
            IObservableReadOnlyList<A> readOnlyList)
        {
            if (readOnlyList == null) throw new ArgumentNullException(nameof(readOnlyList));
            Assert.Equal(0, readOnlyList.Count);
        }
    }
}
