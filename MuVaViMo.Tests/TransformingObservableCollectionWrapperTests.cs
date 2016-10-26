using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xunit;

namespace MuVaViMo.Tests
{
    public class TransformingObservableCollectionWrapperTests
    {
        class A {}

        class B
        {
            public A A { get; set; }
        }

        ObservableCollection<A> FilledSourceCollection => new ObservableCollection<A> {new A(), new A(), new A()};

        [Fact]
        public void EmptySourceCollection()
        {
            //Arrange
            ObservableCollection<A> filledCollection = new ObservableCollection<A>();
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });

            //Act


            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        [Fact]
        public void FilledCollection()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });

            //Act


            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        [Fact]
        public void OnAddition()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            A addedA = new A();
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });
            collectionWrapper.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(collectionWrapper.Count - 1, args.NewStartingIndex);
                Assert.Same(addedA, ((B)args.NewItems[0]).A);
            };

            //Act
            filledCollection.Add(addedA);

            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        [Fact]
        public void OnInsert()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            A insertedA = new A();
            int insertIndex = 1;
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });
            collectionWrapper.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Add, args.Action);
                Assert.Equal(insertIndex, args.NewStartingIndex);
                Assert.Same(insertedA, ((B)args.NewItems[0]).A);
            };

            //Act
            filledCollection.Insert(insertIndex, insertedA);

            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        [Fact]
        public void OnRemoveAt()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            int removedIndex = 1;
            A removedA = filledCollection[removedIndex];
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });
            collectionWrapper.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, ((B)args.OldItems[0]).A);
            };

            //Act
            filledCollection.RemoveAt(removedIndex);

            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        [Fact]
        public void OnRemove()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            int removedIndex = 1;
            A removedA = new A();
            filledCollection.Insert(removedIndex, removedA);
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });
            collectionWrapper.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Remove, args.Action);
                Assert.Equal(removedIndex, args.OldStartingIndex);
                Assert.Same(removedA, ((B)args.OldItems[0]).A);
            };

            //Act
            filledCollection.RemoveAt(removedIndex);

            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        [Fact]
        public void OnReset()
        {
            //Arrange
            ObservableCollection<A> filledCollection = FilledSourceCollection;
            TransformingObservableCollectionWrapper<A, B> collectionWrapper =
                new TransformingObservableCollectionWrapper<A, B>(filledCollection, a => new B { A = a });
            collectionWrapper.CollectionChanged += (sender, args) =>
            {
                Assert.Equal(NotifyCollectionChangedAction.Reset, args.Action);
            };

            //Act
            filledCollection.Clear();

            //Assert
            StandardCheck(filledCollection, collectionWrapper);
        }

        private static void StandardCheck(ObservableCollection<A> filledCollection,
                                          TransformingObservableCollectionWrapper<A, B> collectionWrapper)
        {
            Assert.Equal(filledCollection.Count, collectionWrapper.Count);
            int index = 0;
            foreach(B b in collectionWrapper)
            {
                Assert.Same(b.A, filledCollection[index++]);
            }
        }
    }
}
