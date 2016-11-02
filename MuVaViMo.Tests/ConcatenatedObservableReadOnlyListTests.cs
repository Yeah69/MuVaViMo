using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xunit;

namespace MuVaViMo.Tests
{
    public class ConcatenatedObservableReadOnlyListTests
    {
        abstract class Super {}

        class A : Super {}

        class B : Super {}

        ObservableCollection<A> FilledCollectionA => new ObservableCollection<A> { new A(), new A(), new A(), new A(), new A(), new A(), new A() };
        ObservableCollection<B> FilledCollectionB => new ObservableCollection<B> { new B(), new B(), new B(), new B(), new B() };

        [Fact]
        public void EmptySourceCollection()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = new ObservableCollection<A>();
            ObservableCollection<B> sourceCollectionB = new ObservableCollection<B>();
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);

            //Act


            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void FilledCollection()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);

            //Act


            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnAdditionA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            A addedA = new A();
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(sourceCollectionA.Count - 1, args.NewStartingIndex);
                Assert.Same(addedA, args.NewItems[0]);
            };

            //Act
            sourceCollectionA.Add(addedA);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnAdditionB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            B addedB = new B();
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(readOnlyList.Count - 1, args.NewStartingIndex);
                Assert.Same(addedB, args.NewItems[0]);
            };

            //Act
            sourceCollectionB.Add(addedB);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnInsertA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            A insertedA = new A();
            int insertIndex = 1;
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(insertIndex, args.NewStartingIndex);
                Assert.Same(insertedA, args.NewItems[0]);
            };

            //Act
            sourceCollectionA.Insert(insertIndex, insertedA);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnInsertB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            B insertedB = new B();
            int insertIndex = 1;
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(sourceCollectionA.Count + insertIndex, args.NewStartingIndex);
                Assert.Same(insertedB, args.NewItems[0]);
            };

            //Act
            sourceCollectionB.Insert(insertIndex, insertedB);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnRemoveAtA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            int removedIndex = 1;
            A removedA = sourceCollectionA[removedIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, args.OldItems[0]);
            };

            //Act
            sourceCollectionA.RemoveAt(removedIndex);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnRemoveAtB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            int removedIndex = 1;
            B removedB = sourceCollectionB[removedIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(sourceCollectionA.Count + removedIndex, args.OldStartingIndex);
                Assert.Same(removedB, args.OldItems[0]);
            };

            //Act
            sourceCollectionB.RemoveAt(removedIndex);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnRemoveA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            int removedIndex = 1;
            A removedA = new A();
            sourceCollectionA.Insert(removedIndex, removedA);
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, args.OldItems[0]);
            };

            //Act
            sourceCollectionA.RemoveAt(removedIndex);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnRemoveB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            int removedIndex = 1;
            B removedB = new B();
            sourceCollectionB.Insert(removedIndex, removedB);
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(sourceCollectionA.Count + removedIndex, args.OldStartingIndex);
                Assert.Same(removedB, args.OldItems[0]);
            };

            //Act
            sourceCollectionB.RemoveAt(removedIndex);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnResetA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Reset, args.Action);
            };

            //Act
            sourceCollectionA.Clear();

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnResetB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Reset, args.Action);
            };

            //Act
            sourceCollectionB.Clear();

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnReplaceA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            A replacingA = new A();
            int replaceIndex = 1;
            A replacedA = (A)readOnlyList[replaceIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Replace, args.Action);
                Assert.Equal(replaceIndex, args.OldStartingIndex);
                Assert.Equal(replaceIndex, args.NewStartingIndex);
                Assert.Same(replacingA, args.NewItems[0]);
            };

            //Act
            sourceCollectionA[replaceIndex] = replacingA;

            //Assert
            Assert.NotSame(replacedA, readOnlyList[replaceIndex]);
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnReplaceB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            B replacingB = new B();
            int replaceIndex = 1;
            B replacedB = (B)readOnlyList[sourceCollectionA.Count + replaceIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Replace, args.Action);
                Assert.Equal(sourceCollectionA.Count + replaceIndex, args.OldStartingIndex);
                Assert.Equal(sourceCollectionA.Count + replaceIndex, args.NewStartingIndex);
                Assert.Same(replacingB, args.NewItems[0]);
            };

            //Act
            sourceCollectionB[replaceIndex] = replacingB;

            //Assert
            Assert.NotSame(replacedB, readOnlyList[replaceIndex]);
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnMoveA()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            int moveFromIndex = 1;
            int moveToIndex = 3;
            A movedA = sourceCollectionA[moveFromIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Move, args.Action);
                Assert.Equal(moveFromIndex, args.OldStartingIndex);
                Assert.Equal(moveToIndex, args.NewStartingIndex);
                Assert.Same(movedA, args.NewItems[0]);
            };

            //Act
            sourceCollectionA.Move(moveFromIndex, moveToIndex);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        [Fact]
        public void OnMoveB()
        {
            //Arrange
            ObservableCollection<A> sourceCollectionA = FilledCollectionA;
            ObservableCollection<B> sourceCollectionB = FilledCollectionB;
            WrappingObservableReadOnlyList<A> wrappedCollectionA = new WrappingObservableReadOnlyList<A>(sourceCollectionA);
            WrappingObservableReadOnlyList<B> wrappedCollectionB = new WrappingObservableReadOnlyList<B>(sourceCollectionB);
            ConcatenatingObservableReadOnlyList<Super> readOnlyList =
                new ConcatenatingObservableReadOnlyList<Super>(wrappedCollectionA, wrappedCollectionB);
            int moveFromIndex = 1;
            int moveToIndex = 3;
            B movedB = sourceCollectionB[moveFromIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Move, args.Action);
                Assert.Equal(sourceCollectionA.Count + moveFromIndex, args.OldStartingIndex);
                Assert.Equal(sourceCollectionA.Count + moveToIndex, args.NewStartingIndex);
                Assert.Same(movedB, args.NewItems[0]);
            };

            //Act
            sourceCollectionB.Move(moveFromIndex, moveToIndex);

            //Assert
            StandardCheck(sourceCollectionA, sourceCollectionB, readOnlyList);
        }

        private static void StandardCheck(ObservableCollection<A> filledCollectionA, ObservableCollection<B> filledCollectionB,
                                          ConcatenatingObservableReadOnlyList<Super> readOnlyList)
        {
            Assert.Equal(filledCollectionA.Count + filledCollectionB.Count, readOnlyList.Count);
            int index = 0;
            foreach(A a in filledCollectionA)
            {
                Assert.Same(a, readOnlyList[index++]);
            }
            foreach(B b in filledCollectionB)
            {
                Assert.Same(b, readOnlyList[index++]);
            }
        }
    }
}
