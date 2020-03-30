using System;
using System.Collections.Generic;
using System.Linq;

using Meekys.OData.Expressions;

using Xunit;

namespace Meekys.OData.Tests.Expressions
{
    public class BufferedEnumeratorTests : IDisposable
    {
        private BufferedEnumerator<int> _enumerator;
        private int _fetchCount;
        private bool _fetchEndOfList;
        private bool _disposed;

        public BufferedEnumeratorTests()
        {
            var range = TestRange(0, 100).GetEnumerator();

            _enumerator = new BufferedEnumerator<int>(range);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _enumerator.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        [Fact]
        public void Test_Enumeration_Normal()
        {
            for (var i = 0; i < 100; i++)
            {
                // Arrange
                _fetchCount = 0;

                // Act + Assert
                Assert.True(_enumerator.MoveNext(), $"MoveNext() failed for i={i}");
                Assert.Equal(1, _fetchCount);
                Assert.Equal(i, _enumerator.Current);
            }

            Assert.False(_fetchEndOfList);

            Assert.False(_enumerator.MoveNext());
            Assert.True(_fetchEndOfList);
        }

        [Fact]
        public void Test_Peek_Normal_And_Past_Buffer()
        {
            for (var i = 0; i < 5; i++)
            {
                // Arrange
                _fetchCount = 0;

                // Act + Assert
                Assert.True(_enumerator.CanPeek(i));
                Assert.Equal(i, _enumerator.Peek(i));
                Assert.Equal(1, _fetchCount);
            }

            // Arrange
            _fetchCount = 0;

            // Act + Assert
            var canPeekEx = Assert.Throws<ArgumentException>(() => (object)_enumerator.CanPeek(5));
            Assert.Equal("Offset is larger than buffer size (Parameter 'offset')", canPeekEx.Message);

            var peekEx = Assert.Throws<ArgumentException>(() => (object)_enumerator.Peek(5));
            Assert.Equal("Offset is larger than buffer size (Parameter 'offset')", peekEx.Message);

            Assert.Equal(0, _fetchCount);
            Assert.False(_fetchEndOfList);
        }

        [Fact]
        public void Test_Enumeration_Large_Peek()
        {
            // Arrange
            _enumerator.MoveNext();
            _fetchCount = 0;

            // Act + Assert
            Assert.Equal(0, _enumerator.Current);
            Assert.Equal(3, _enumerator.Peek(2)); // Peeking 0-2 (Count of 3)
            Assert.Equal(3, _fetchCount);
        }

        [Fact]
        public void Test_Peek_At_Near_And_End_Of_Iterator()
        {
            // Arrange
            for (var i = 0; i < 99; i++)
            {
                _enumerator.MoveNext();
            }

            _fetchCount = 0;

            // Act + Assert
            Assert.True(_enumerator.CanPeek(0));
            Assert.Equal(99, _enumerator.Peek(0));
            Assert.Equal(1, _fetchCount);

            // Arrange
            _fetchCount = 0;

            // Act + Assert
            Assert.False(_enumerator.CanPeek(1));
            var peekEx = Assert.Throws<IndexOutOfRangeException>(() => (object)_enumerator.Peek(1));
            Assert.Equal("No more items to retrieve", peekEx.Message);
            Assert.Equal(0, _fetchCount);
        }

        [Fact]
        public void Test_History_At_Near_And_Start_Of_Iterator()
        {
            // Arrange
            _enumerator.MoveNext(); // Into current
            _enumerator.MoveNext(); // Into history
            _fetchCount = 0;

            // Act
            Assert.True(_enumerator.HasPrevious(0));
            Assert.Equal(0, _enumerator.Previous(0));

            Assert.False(_enumerator.HasPrevious(1));
            var prevEx = Assert.Throws<IndexOutOfRangeException>(() => (object)_enumerator.Previous(1));
            Assert.Equal("No such item in history", prevEx.Message);
            Assert.Equal(0, _fetchCount);
        }

        [Fact]
        public void Test_History_At_End_Of_Iterator()
        {
            // Arrange
            for (var i = 0; i < 100; i++)
            {
                _enumerator.MoveNext();
            }

            _enumerator.MoveNext();
            _fetchCount = 0;

            // Act + Assert
            Assert.False(_enumerator.CanPeek());

            for (var i = 0; i < 5; i++)
            {
                Assert.True(_enumerator.HasPrevious(i));
                Assert.Equal(99 - i, _enumerator.Previous(i));
                Assert.Equal(0, _fetchCount);
            }

            var hasPrevEx = Assert.Throws<ArgumentException>(() => (object)_enumerator.HasPrevious(5));
            Assert.Equal("Offset is larger than buffer size (Parameter 'offset')", hasPrevEx.Message);

            var prevEx = Assert.Throws<ArgumentException>(() => (object)_enumerator.Previous(5));
            Assert.Equal("Offset is larger than buffer size (Parameter 'offset')", prevEx.Message);

            Assert.Equal(0, _fetchCount);
        }

        private IEnumerable<int> TestRange(int start, int count)
        {
            for (var i = 0; i < count; i++)
            {
                _fetchCount++;

                yield return start + i;
            }

            _fetchEndOfList = true;
        }
    }
}