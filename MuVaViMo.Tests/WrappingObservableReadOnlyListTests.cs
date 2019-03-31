using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xunit;

namespace MuVaViMo.Tests
{
    public class WrappingObservableReadOnlyListTests
    {
        class A { }

        ObservableCollection<A> FilledSourceCollection => new ObservableCollection<A> { new A(), new A(), new A(), new A(), new A(), new A() };

        [Fact]
        public void EmptySourceCollection()
        {
            //Arrange
            ObservableCollection<A> filledCollection = new ObservableCollection<A>();
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);

                //Act


            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void FilledCollection()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);

            //Act


            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnAddition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            A addedA = new A();
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(readOnlyList.Count - 1, args.NewStartingIndex);
                Assert.Same(addedA, args.NewItems[0]);
            };

            //Act
            filledCollection.Add(addedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnInsert()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            A insertedA = new A();
            int insertIndex = 1;
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(insertIndex, args.NewStartingIndex);
                Assert.Same(insertedA, args.NewItems[0]);
            };

            //Act
            filledCollection.Insert(insertIndex, insertedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnRemoveAt()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, args.OldItems[0]);
            };

            //Act
            filledCollection.RemoveAt(removedIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnRemove()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];
            filledCollection.Insert(removedIndex, removedA);
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, args.OldItems[0]);
            };

            //Act
            filledCollection.Remove(removedA);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnReset()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Reset, args.Action);
            };

            //Act
            filledCollection.Clear();

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnReplace()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            A replacingA = new A();
            int replaceIndex = 1;
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Replace, args.Action);
                Assert.Equal(replaceIndex, args.OldStartingIndex);
                Assert.Equal(replaceIndex, args.NewStartingIndex);
                Assert.Same(replacingA, args.NewItems[0]);
            };

            //Act
            filledCollection[replaceIndex] = replacingA;

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnMove()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            WrappingObservableReadOnlyList<A> readOnlyList = new WrappingObservableReadOnlyList<A>(filledCollection);
            int moveFromIndex = 1;
            int moveToIndex = 3;
            A movedA = filledCollection[moveFromIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Move, args.Action);
                Assert.Equal(moveFromIndex, args.OldStartingIndex);
                Assert.Equal(moveToIndex, args.NewStartingIndex);
                Assert.Same(movedA, args.NewItems[0]);
            };

            //Act
            filledCollection.Move(moveFromIndex, moveToIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        private static void StandardCheck(ObservableCollection<A> filledCollection,
                                          WrappingObservableReadOnlyList<A> readOnlyList)
        {
            Assert.Equal(filledCollection.Count, readOnlyList.Count);
            int index = 0;
            foreach (A a in readOnlyList)
            {
                Assert.Same(a, filledCollection[index++]);
            }
        }
    }
}
