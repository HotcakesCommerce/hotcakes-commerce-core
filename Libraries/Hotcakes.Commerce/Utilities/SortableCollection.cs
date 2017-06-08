#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Hotcakes.Commerce.Utilities
{
    public class SortableCollection<T> : CollectionBase
    {
        public SortableCollection()
        {
        }

        public SortableCollection(IEnumerable<T> enumerator)
        {
            foreach (var item in enumerator)
            {
                var temp = item;
                Add(temp);
            }
        }

        public T this[int index]
        {
            get { return (T) List[index]; }
            set { List[index] = value; }
        }

        public void Sort(string sortExpression, SortDirection direction)
        {
            InnerList.Sort(new SortableCollectionComparer(sortExpression, direction));
        }

        public void Sort(string sortExpression)
        {
            InnerList.Sort(new SortableCollectionComparer(sortExpression));
        }

        public virtual int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public virtual int Add(T item)
        {
            return List.Add(item);
        }

        public virtual void Remove(T item)
        {
            List.Remove(item);
        }

        public virtual void CopyTo(Array a, int index)
        {
            List.CopyTo(a, index);
        }

        public virtual void AddRange(SortableCollection<T> collection)
        {
            InnerList.AddRange(collection);
        }

        public virtual void AddRange(T[] collection)
        {
            InnerList.AddRange(collection);
        }

        public virtual bool Contains(T item)
        {
            return List.Contains(item);
        }

        public virtual void Insert(int index, T item)
        {
            List.Insert(index, item);
        }

        public string ToXml()
        {
            var result = string.Empty;

            try
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(GetType());
                xs.Serialize(sw, this);
                result = sw.ToString();
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                result = string.Empty;
            }

            return result;
        }

        public static SortableCollection<T> FromXml(string data)
        {
            var result = new SortableCollection<T>();

            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var tr = new StringReader(data);
                    var xs = new XmlSerializer(result.GetType());
                    result = (SortableCollection<T>) xs.Deserialize(tr);
                    if (result == null)
                    {
                        result = new SortableCollection<T>();
                    }
                }
                catch (Exception ex)
                {
                    EventLog.LogEvent(ex);
                    result = new SortableCollection<T>();
                }
            }

            return result;
        }

        public List<T> ToList()
        {
            var result = new List<T>();

            foreach (T item in List)
            {
                result.Add(item);
            }
            return result;
        }
    }
}