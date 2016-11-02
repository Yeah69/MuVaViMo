using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xunit;

namespace MuVaViMo.Tests
{
    public class TransformingObservableReadOnlyListTests
    {
        class A {}

        class B
        {
            public A A { get; set; }
        }

        ObservableCollection<A> FilledSourceCollection => new ObservableCollection<A> {new A(), new A(), new A(), new A(), new A(), new A() };

        [Fact]
        public void EmptySourceCollection()
        {
            //Arrange
            ObservableCollection<A> filledCollection = new ObservableCollection<A>();
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });

            //Act


            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void FilledCollection()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });

            //Act


            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnAddition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            A addedA = new A();
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(readOnlyList.Count - 1, args.NewStartingIndex);
                Assert.Same(addedA, ((B)args.NewItems[0]).A);
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
            A insertedA = new A();
            int insertIndex = 1;
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(insertIndex, args.NewStartingIndex);
                Assert.Same(insertedA, ((B)args.NewItems[0]).A);
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
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, ((B)args.OldItems[0]).A);
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
            int removedIndex = 1;
            A removedA = new A();
            filledCollection.Insert(removedIndex, removedA);
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, ((B)args.OldItems[0]).A);
            };

            //Act
            filledCollection.RemoveAt(removedIndex);

            //Assert
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnReset()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
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
            A replacingA = new A();
            int replaceIndex = 1;
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
            B replacedB = readOnlyList[replaceIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Replace, args.Action);
                Assert.Equal(replaceIndex, args.OldStartingIndex);
                Assert.Equal(replaceIndex, args.NewStartingIndex);
                Assert.Same(replacingA, ((B)args.NewItems[0]).A);
            };

            //Act
            filledCollection[replaceIndex] = replacingA;

            //Assert
            Assert.NotSame(replacedB, readOnlyList[replaceIndex]); //B should be actually replaced. No recycling of B objects!
            StandardCheck(filledCollection, readOnlyList);
        }

        [Fact]
        public void OnMove()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            int moveFromIndex = 1;
            int moveToIndex = 3;
            A movedA = filledCollection[moveFromIndex];
            TransformingObservableReadOnlyList<A, B> readOnlyList =
                new TransformingObservableReadOnlyList<A, B>(filledCollection, a => new B { A = a });
            B movedB = readOnlyList[moveFromIndex];
            readOnlyList.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Move, args.Action);
                Assert.Equal(moveFromIndex, args.OldStartingIndex);
                Assert.Equal(moveToIndex, args.NewStartingIndex);
                Assert.Same(movedA, ((B)args.NewItems[0]).A);
            };

            //Act
            filledCollection.Move(moveFromIndex, moveToIndex);

            //Assert
            Assert.Same(movedB, readOnlyList[moveToIndex]); //Check that the corresponding B object was moved and no new instance was created
            StandardCheck(filledCollection, readOnlyList);
        }

        private static void StandardCheck(ObservableCollection<A> filledCollection,
                                          TransformingObservableReadOnlyList<A, B> readOnlyList)
        {
            Assert.Equal(filledCollection.Count, readOnlyList.Count);
            int index = 0;
            foreach(B b in readOnlyList)
            {
                Assert.Same(b.A, filledCollection[index++]);
            }
        }
    }
}
