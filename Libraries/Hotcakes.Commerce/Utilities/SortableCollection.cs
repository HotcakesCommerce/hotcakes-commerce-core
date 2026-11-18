#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>
    /// Provides a legacy collection with typed access, property-based sorting, and XML conversion.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public class SortableCollection<T> : CollectionBase
    {
        /// <summary>Initializes an empty collection.</summary>
        public SortableCollection()
        {
        }

        /// <summary>Initializes a collection containing the items from the specified sequence.</summary>
        /// <param name="enumerator">The sequence whose items are copied into the collection.</param>
        public SortableCollection(IEnumerable<T> enumerator)
        {
            foreach (var item in enumerator)
            {
                var temp = item;
                Add(temp);
            }
        }

        /// <summary>Gets or sets the item at the specified zero-based index.</summary>
        /// <param name="index">The zero-based index of the item.</param>
        /// <value>The item at <paramref name="index"/>.</value>
        public T this[int index]
        {
            get { return (T) List[index]; }
            set { List[index] = value; }
        }

        /// <summary>Sorts the collection by a public property in the specified direction.</summary>
        /// <param name="sortExpression">The name of the property used for comparison.</param>
        /// <param name="direction">The requested sort direction.</param>
        /// <seealso cref="SortableCollectionComparer"/>
        public void Sort(string sortExpression, SortDirection direction)
        {
            InnerList.Sort(new SortableCollectionComparer(sortExpression, direction));
        }

        /// <summary>Sorts the collection by a public property in ascending order.</summary>
        /// <param name="sortExpression">The name of the property used for comparison.</param>
        /// <seealso cref="Sort(string, SortDirection)"/>
        public void Sort(string sortExpression)
        {
            InnerList.Sort(new SortableCollectionComparer(sortExpression));
        }

        /// <summary>Returns the zero-based index of an item, or <c>-1</c> when it is not present.</summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The item's zero-based index, or <c>-1</c>.</returns>
        public virtual int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        /// <summary>Adds an item and returns its zero-based index.</summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index at which the item was added.</returns>
        public virtual int Add(T item)
        {
            return List.Add(item);
        }

        /// <summary>Removes the first matching item from the collection.</summary>
        /// <param name="item">The item to remove.</param>
        public virtual void Remove(T item)
        {
            List.Remove(item);
        }

        /// <summary>Copies the collection to an array beginning at the specified array index.</summary>
        /// <param name="a">The destination array.</param>
        /// <param name="index">The zero-based destination index.</param>
        public virtual void CopyTo(Array a, int index)
        {
            List.CopyTo(a, index);
        }

        /// <summary>Adds all items from another sortable collection.</summary>
        /// <param name="collection">The collection whose items are appended.</param>
        public virtual void AddRange(SortableCollection<T> collection)
        {
            InnerList.AddRange(collection);
        }

        /// <summary>Adds all items from an array.</summary>
        /// <param name="collection">The array whose items are appended.</param>
        public virtual void AddRange(T[] collection)
        {
            InnerList.AddRange(collection);
        }

        /// <summary>Determines whether the collection contains an item.</summary>
        /// <param name="item">The item to locate.</param>
        /// <returns><see langword="true"/> when the item is present; otherwise, <see langword="false"/>.</returns>
        public virtual bool Contains(T item)
        {
            return List.Contains(item);
        }

        /// <summary>Inserts an item at the specified zero-based index.</summary>
        /// <param name="index">The zero-based insertion index.</param>
        /// <param name="item">The item to insert.</param>
        public virtual void Insert(int index, T item)
        {
            List.Insert(index, item);
        }

        /// <summary>Serializes this collection to XML.</summary>
        /// <returns>The serialized XML, or <see cref="string.Empty"/> when serialization fails.</returns>
        /// <remarks>Serialization failures are logged through <see cref="EventLog"/>.</remarks>
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

        /// <summary>Deserializes a sortable collection from XML.</summary>
        /// <param name="data">The XML text to deserialize.</param>
        /// <returns>The deserialized collection, or an empty collection when the input is empty or deserialization fails.</returns>
        /// <remarks>Deserialization failures are logged through <see cref="EventLog"/>.</remarks>
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

        /// <summary>Copies the collection's items to a new generic list.</summary>
        /// <returns>A new list containing the collection's items in their current order.</returns>
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
