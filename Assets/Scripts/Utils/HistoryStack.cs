using System;

namespace Utils
{
    public class HistoryStack<T>
    {
        private T[] itemArray = Array.Empty<T>();
        private readonly T defaultItem;
        private ArrayCursor headIndex; // point **at** head
        private ArrayCursor tailIndex; // point **at** tail
        
        public int Count { get; private set; } = 0;
        public int Capacity
        {
            get => itemArray.Length;
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(Capacity), "Capacity must be greater than zero.");
                if (itemArray.Length == value) return;
                
                var oldArray = itemArray;
                var newArray = new T[value];
                if (oldArray.Length < value)
                {
                    for (var i = oldArray.Length - 1; i >= 0; i--) newArray[i] = oldArray[i];
                    tailIndex.SetRange(newArray.Length);
                    headIndex.SetRange(newArray.Length);
                }
                else
                {
                    var endPosition = Math.Max(value - 1 - Count, 0);
                    for (var i = value - 1; i >= endPosition; i--) newArray[i] = oldArray[tailIndex--];

                    tailIndex.SetRange(newArray.Length);
                    tailIndex.Set(value-1);
                    headIndex.SetRange(newArray.Length);
                    headIndex.Set(endPosition);
                }
                itemArray = newArray;
            }
        }

        public void Push(T item)
        {
            itemArray[++tailIndex] = item;
            if (Count == Capacity) headIndex++; else Count++;
        }
        public T Pop()
        {
            if (Count == 0) return defaultItem; else Count--;
            return itemArray[tailIndex--];
        }
        public T Peek() => Count == 0 ? defaultItem : itemArray[tailIndex];
        public void Clear()
        {
            while (tailIndex != headIndex) itemArray[headIndex++] = default(T);
            tailIndex.Set(0);
            headIndex.Set(0);
            Count = 0;
        }
        
        public HistoryStack(T defaultItem, int capacity=64)
        {
            this.defaultItem = defaultItem;
            this.Capacity = capacity;
        }
    }

    internal struct ArrayCursor
    {
        private int value;
        private int maxValue;

        private static int Mod(int a, int b) => ((a % b) + b) % b;
        public static ArrayCursor operator++(ArrayCursor self) => new ArrayCursor(self.value == self.maxValue - 1 ? 0 : self.value + 1, self.maxValue);
        public static ArrayCursor operator--(ArrayCursor self) => new ArrayCursor(self.value == 0 ? self.maxValue - 1 : self.value - 1, self.maxValue);
        
        public static implicit operator int(ArrayCursor source) => source.value;
        public void SetRange(int maxVal)
        {
            if (maxVal < 0) throw new ArgumentOutOfRangeException(nameof(maxVal), "Max value must be greater than zero.");
            maxValue = maxVal;
            value = Mod(value, maxVal);
        }
        public void Set(int val) => value = Mod(val, maxValue);

        private ArrayCursor(int value, int maxValue)
        {
            this.value = value;
            this.maxValue = maxValue;
        }
    }
}

