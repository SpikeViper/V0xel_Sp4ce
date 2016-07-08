using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BK.Util
{
    internal class ThreadDetails<Key,Value> where Key: IComparable<Key>
    {
        string threadId;
        Key lastSearchedItem;
        int count;

        int currentIndex;
        Key keyForThread;

        public void StoreCurrentIndex(Key KeyToStore, int CurrIndex)
        {
            currentIndex = CurrIndex;
            keyForThread = KeyToStore;
        }

        public int GetStoredIndex(ref Key KeyStored)
        {
            KeyStored = keyForThread;
            return currentIndex;
        }

        private void Check(Key KeyParam)
        {
            if (0 != KeyParam.CompareTo(lastSearchedItem))
            {
                lastSearchedItem = KeyParam;
                ResetCount();
            }
        }

        public ThreadDetails(string ThreadId, Key LastSearchedItem, int Count)
        {
            threadId = ThreadId;
            lastSearchedItem = LastSearchedItem;
            count = Count;
            currentIndex = -1;
        }

        public string ThreadId
        {
            get
            {
                return threadId;
            }
        }

        public Key LastSearchedItem
        {
            get
            {
                return lastSearchedItem;
            }

            set
            {
                lastSearchedItem = value;
            }
        }

        public int GetCurrentCount(Key KeyParam)
        {
            Check(KeyParam);
            return count;
        }

        public void IncrementCurrentCount(Key KeyParam)
        {
            Check(KeyParam);
            count++;
        }

        public void ResetCount()
        {
            count = -1;
        }

        internal int Counter
        {
            get
            {
                return count;
            }
        }
    }
}
