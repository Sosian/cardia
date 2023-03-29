using System;
using System.Collections;
using System.Collections.Generic;

namespace MGT.Utilities.Collections
{
  public class CircularBuffer<T> : IEnumerable<T>, IEnumerable
  {
    private T[] _buffer;
    private int _latestIndex = -1;
    private bool bufferFull;

    public int BufferSize { get; private set; }

    public int Count => this.bufferFull ? this.BufferSize : this._latestIndex + 1;

    public CircularBuffer(int size)
    {
      this.BufferSize = size;
      this._latestIndex = -1;
      this._buffer = new T[this.BufferSize];
    }

    public void Add(T item)
    {
      ++this._latestIndex;
      if (this._latestIndex == this.BufferSize)
      {
        this.bufferFull = true;
        this._latestIndex = 0;
      }
      this._buffer[this._latestIndex] = item;
    }

    public void Clear()
    {
      this._buffer = new T[this.BufferSize];
      this._latestIndex = -1;
    }

    public IEnumerator<T> GetEnumerator()
    {
      if (this._latestIndex >= 0)
      {
        int currentIndex = this._latestIndex;
        int loopCounter = 0;
        while (loopCounter != this.BufferSize)
        {
          ++loopCounter;
          yield return this._buffer[currentIndex];
          --currentIndex;
          if (currentIndex < 0)
          {
            if (!this.bufferFull)
              break;
            currentIndex = this.BufferSize - 1;
          }
        }
      }
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public T this[int i]
    {
      get
      {
        if (i >= this.BufferSize)
          throw new ArgumentOutOfRangeException();
        int index = this._latestIndex - i;
        if (index < 0)
          index = this.BufferSize + index;
        return this._buffer[index];
      }
      set
      {
        if (i >= this.BufferSize)
          throw new ArgumentOutOfRangeException();
        int index = this._latestIndex - i;
        if (index < 0)
          index = this.BufferSize - index;
        this._buffer[index] = value;
      }
    }

    public T[] ToArray()
    {
      if (this._latestIndex < 0)
        return this._buffer;
      T[] array = new T[this.BufferSize];
      for (int index1 = 0; index1 < array.Length; ++index1)
      {
        int index2 = this._latestIndex - index1;
        if (index2 < 0)
          index2 = this._buffer.Length + index2;
        array[index1] = this._buffer[index2];
      }
      return array;
    }
  }
}
