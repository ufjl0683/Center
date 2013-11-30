using System;
using System.Collections.Generic;
using System.Text;

namespace Execution
{
    [Serializable]
    abstract internal class MyList<T>
    {
        private List<T> tlist;
        public MyList()
        {
            tlist = new List<T>();
        }

        public void Add(T item)
        {
            tlist.Add(item);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            tlist.AddRange(collection);
        }

        public void Clear()
        {
            tlist.Clear();
        }

        public bool Contains(T obj)
        {
            return tlist.Contains(obj);
        }

        public int Count()
        {
            return tlist.Count;
        }
       
        public bool Remove(T obj)
        {
            return tlist.Remove(obj);
        }

        public void RemoveAt(int index)
        {
            tlist.RemoveAt(index);
        }

        public T getAt(int index)
        {
            return tlist[index];
        }

        public List<T> getList()
        {
            return tlist;
        }

        abstract public T Find(object sender);
    }
}
