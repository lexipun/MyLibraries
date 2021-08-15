using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace DotNetCore.Collections
{
    public class List<T> : ICloneable, ICollection<T>, IComparable<T>, IComparer<T>, IEnumerable<T>, IDisposable, IConvertible, IList<T>
    {
        private T[] _array;
        private int _size;
        private int _count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                {
                    throw new IndexOutOfRangeException("index out of the range of List");
                }

                return _array[index];

            }
            set
            {

                if (index < 0 || index >= _size)
                {
                    throw new IndexOutOfRangeException("index out of the range of List");
                }

                _array[index] = value;
            }
        }

        public int Count => _count;

        public bool IsReadOnly => false;

        public List()
        {
            _size = 1;
            _array = new T[_size];
            _count = 0;
        }
        public List(T element)
        {
            _size = 1;
            _array = new T[_size];
            _array[0] = element;
            _count = 1;
        }
        public List(ICollection<T> elements)
        {
            _size = elements.Count;
            _array = new T[_size];
            _count = _size;

            elements.CopyTo(_array, 0);
        }

        public void Add(T item)
        {
            if (item is null)
            {
                throw new ArgumentNullException("passed null to adding in List");
            }

            if (_size > _count)
            {
                _array[_count++] = item;
                return;
            }

            _size *= 2;

            if (_size > int.MaxValue)
            {
                _size = int.MaxValue;
            }

            if (_count == int.MaxValue)
            {
                throw new OutOfMemoryException("over much count of elements");
            }

            Array.Resize(ref _array, _size);

            _array[_count++] = item;
        }
        public void AddRange(IEnumerable<T> enumerable)
        {
            if (enumerable is null)
            {
                return;
            }

            ICollection<T> collection = enumerable as ICollection<T>;

            if (collection is null)
            {
                using (IEnumerator<T> enumerator = enumerable.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Add(enumerator.Current);
                    }
                }
                return;
            }

            int newCount = _count + collection.Count - 1;

            if(newCount > int.MaxValue)
            {
                throw new OutOfMemoryException("over much count of elements");
            }

            if (newCount >= _size)
            {
                _size *= 2;

                if (newCount > _size)
                {
                    _size = newCount;
                    Array.Resize(ref _array, newCount);
                }
                else
                {
                    Array.Resize(ref _array, _size);
                }
            }

            collection.CopyTo(_array, _count);

        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public int Compare([AllowNull] T x, [AllowNull] T y)
        {
            throw new NotImplementedException();
        }

        public int CompareTo([AllowNull] T other)
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
