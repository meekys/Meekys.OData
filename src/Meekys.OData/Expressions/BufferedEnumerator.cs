using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Meekys.OData.Expressions
{
    public class BufferedEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> _enumerator;
        private int _maxSize;

        private bool _hasCurrent;
        private bool _endOfIterator;

        private T _current;
        private List<T> _previous;
        private List<T> _next;

        private bool _disposed = false; // To detect redundant calls

        public BufferedEnumerator(IEnumerator<T> enumerator, int maxSize = 5)
        {
            _enumerator = enumerator;
            _maxSize = maxSize;

            ResetState();
        }

        public T Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public bool MoveNext()
        {
            if (_hasCurrent)
                _previous.Insert(0, _current);

            if (_previous.Count > _maxSize)
                _previous.RemoveAt(_maxSize);

            _hasCurrent = CanPeek();

            _current = _hasCurrent ? _next[0] : default(T);

            if (_hasCurrent)
                _next.RemoveAt(0);

            return _hasCurrent;
        }

        public bool CanPeek()
        {
            return CanPeek(0);
        }

        public bool CanPeek(int offset)
        {
            if (offset >= _maxSize)
                throw new ArgumentException("Offset is larger than buffer size", nameof(offset));

            if (_next.Count > offset)
                return true;

            if (_endOfIterator)
                return false;

            if (!_hasCurrent)
                _endOfIterator = !FetchNext();

            while (!_endOfIterator && _next.Count <= offset)
                _endOfIterator = !FetchNext();

            return !_endOfIterator;
        }

        private bool FetchNext()
        {
            var hasNext = _enumerator.MoveNext();

            if (hasNext)
                _next.Add(_enumerator.Current);

            return hasNext;
        }

        public T Peek()
        {
            return Peek(0);
        }

        public T Peek(int offset)
        {
            if (!CanPeek(offset))
                throw new IndexOutOfRangeException("No more items to retrieve");

            return _next[offset];
        }

        public bool TryPeek(out T item)
        {
            return TryPeek(0, out item);
        }

        public bool TryPeek(int offset, out T item)
        {
            var canPeek = CanPeek(offset);

            item = canPeek ? Peek(offset) : default(T);

            return canPeek;
        }

        public bool HasPrevious()
        {
            return HasPrevious(0);
        }

        public bool HasPrevious(int offset)
        {
            if (offset >= _maxSize)
                throw new ArgumentException("Offset is larger than buffer size", nameof(offset));

            return offset < _previous.Count;
        }

        public T Previous()
        {
            return Previous(0);
        }

        public T Previous(int offset)
        {
            if (!HasPrevious(offset))
                throw new IndexOutOfRangeException("No such item in history");

            return _previous[offset];
        }

        public void Reset()
        {
            ResetState();

            _enumerator.Reset();
        }

        private void ResetState()
        {
            _hasCurrent = false;
            _endOfIterator = false;
            _current = default(T);
            _previous = new List<T>();
            _next = new List<T>();
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
    }
}