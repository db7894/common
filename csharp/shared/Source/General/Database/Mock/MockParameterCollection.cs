using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;


namespace SharedAssemblies.General.Database.Mock
{
    /// <summary>
    /// A collection of fake data parameters
    /// </summary>
    public class MockParameterCollection : DbParameterCollection
    {
        /// <summary>The synchronization root of the instance.</summary>
        private readonly object _syncRoot = new object();

        /// <summary>The underlying list object of the instance.</summary>
        private readonly List<MockParameter> _list = new List<MockParameter>();


		/// <summary>
		/// Specifies the number of items in the collection.
		/// </summary>
		/// <returns>
		/// The number of items in the collection.
		/// </returns>
		public override int Count
		{
			get { return _list.Count; }
		}


		/// <summary>
		/// Specifies the <see cref="T:System.Object" /> to be used to synchronize 
		/// access to the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Object" /> to be used to synchronize access to 
		/// the <see cref="T:System.Data.Common.DbParameterCollection" />.
		/// </returns>
		public override object SyncRoot
		{
			get { return _syncRoot; }
		}


		/// <summary>
		/// Specifies whether the collection is a fixed size.
		/// </summary>
		/// <returns>
		/// true if the collection is a fixed size; otherwise false.
		/// </returns>
		public override bool IsFixedSize
		{
			get { return false; }
		}


		/// <summary>
		/// Specifies whether the collection is read-only.
		/// </summary>
		/// <returns>
		/// true if the collection is read-only; otherwise false.
		/// </returns>
		public override bool IsReadOnly
		{
			get { return false; }
		}


		/// <summary>
		/// Specifies whether the collection is synchronized.
		/// </summary>
		/// <returns>
		/// true if the collection is synchronized; otherwise false.
		/// </returns>
		public override bool IsSynchronized
		{
			get { return false; }
		}


		/// <summary>
        /// Adds a <see cref="T:System.Data.Common.DbParameter" /> item with the specified 
        /// value to the <see cref="T:System.Data.Common.DbParameterCollection" />.
        /// </summary>
        /// <returns>
        /// The index of the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object in the collection.
        /// </returns>
        /// <param name="value">
        /// The <see cref="P:System.Data.Common.DbParameter.Value" /> of the 
        /// <see cref="T:System.Data.Common.DbParameter" /> to add to the collection.
        /// </param>
        public override int Add(object value)
        {
            _list.Add((MockParameter)value);

            return _list.Count - 1;
        }


        /// <summary>
        /// Indicates whether a <see cref="T:System.Data.Common.DbParameter" /> 
        /// with the specified <see cref="P:System.Data.Common.DbParameter.Value" /> 
        /// is contained in the collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Data.Common.DbParameter" /> is in the 
        /// collection; otherwise false.
        /// </returns>
        /// <param name="value">
        /// The <see cref="P:System.Data.Common.DbParameter.Value" /> of the 
        /// <see cref="T:System.Data.Common.DbParameter" /> to look for in the collection.
        /// </param>
        public override bool Contains(object value)
        {
            // get index of parm with same name and return it or -1 if not found
            for (int i = 0; i < _list.Count; ++i)
            {
                if (Equals(_list[i].Value, value))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Removes all <see cref="T:System.Data.Common.DbParameter" /> values 
        /// from the <see cref="T:System.Data.Common.DbParameterCollection" />.
        /// </summary>
        public override void Clear()
        {
            _list.Clear();
        }


        /// <summary>
        /// Returns the index of the specified <see cref="T:System.Data.Common.DbParameter" /> object.
        /// </summary>
        /// <returns>
        /// The index of the specified <see cref="T:System.Data.Common.DbParameter" /> object.
        /// </returns>
        /// <param name="value">
        /// The <see cref="T:System.Data.Common.DbParameter" /> object in the collection.
        /// </param>
        public override int IndexOf(object value)
        {
            // get index of parm with same name and return it or -1 if not found
            return _list.IndexOf((MockParameter)value);
        }


        /// <summary>
        /// Inserts the specified index of the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object with the specified name into the collection at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index at which to insert the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object.
        /// </param>
        /// <param name="value">
        /// The <see cref="T:System.Data.Common.DbParameter" /> object to insert into 
        /// the collection.
        /// </param>
        public override void Insert(int index, object value)
        {
            _list.Insert(index, (MockParameter)value);
        }


        /// <summary>
        /// Removes the specified <see cref="T:System.Data.Common.DbParameter" /> 
        /// object from the collection.
        /// </summary>
        /// <param name="value">
        /// The <see cref="T:System.Data.Common.DbParameter" /> object to remove.
        /// </param>
        public override void Remove(object value)
        {
            _list.Remove((MockParameter)value);
        }


        /// <summary>
        /// Removes the <see cref="T:System.Data.Common.DbParameter" /> object at the 
        /// specified from the collection.
        /// </summary>
        /// <param name="index">
        /// The index where the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object is located.
        /// </param>
        public override void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }


        /// <summary>
        /// Removes the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object with the specified name from the collection.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object to remove.
        /// </param>
        public override void RemoveAt(string parameterName)
        {
            int pos = IndexOf(parameterName);

            if (pos >= 0)
            {
                _list.RemoveAt(pos);
            }
        }


        /// <summary>
        /// Returns the index of the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object with the specified name.
        /// </summary>
        /// <returns>
        /// The index of the <see cref="T:System.Data.Common.DbParameter" /> object 
        /// with the specified name.
        /// </returns>
        /// <param name="parameterName">
        /// The name of the <see cref="T:System.Data.Common.DbParameter" /> object 
        /// in the collection.
        /// </param>
        public override int IndexOf(string parameterName)
        {
            // get index of parm with same name and return it or -1 if not found
            for (int i = 0; i < _list.Count; ++i)
            {
                if (_list[i].ParameterName.Equals(parameterName))
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Exposes the <see cref="M:System.Collections.IEnumerable.GetEnumerator" /> 
        /// method, which supports a simple iteration over a collection by a .NET Framework data provider.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate 
        /// through the collection.
        /// </returns>
        public override IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }


        /// <summary>
        /// Indicates whether a <see cref="T:System.Data.Common.DbParameter" /> with the 
        /// specified name exists in the collection.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Data.Common.DbParameter" /> is in the collection; 
        /// otherwise false.
        /// </returns>
        /// <param name="value">
        /// The name of the <see cref="T:System.Data.Common.DbParameter" /> to look for in the 
        /// collection.
        /// </param>
        public override bool Contains(string value)
        {
            return (IndexOf(value) >= 0);
        }


        /// <summary>
        /// Copies an array of items to the collection starting at the specified index.
        /// </summary>
        /// <param name="array">
        /// The array of items to copy to the collection.
        /// </param>
        /// <param name="index">
        /// The index in the collection to copy the items.
        /// </param>
        public override void CopyTo(Array array, int index)
        {
            _list.CopyTo((MockParameter[])array, index);
        }


        /// <summary>
        /// Adds an array of items with the specified values to the 
        /// <see cref="T:System.Data.Common.DbParameterCollection" />.
        /// </summary>
        /// <param name="values">
        /// An array of values of type <see cref="T:System.Data.Common.DbParameter" /> 
        /// to add to the collection.
        /// </param>
        public override void AddRange(Array values)
        {
            foreach (MockParameter p in values)
            {
                Add(p);
            }
        }


        /// <summary>
        /// Sets the <see cref="T:System.Data.Common.DbParameter" /> object at the specified 
        /// index to a new value. 
        /// </summary>
        /// <param name="index">
        /// The index where the <see cref="T:System.Data.Common.DbParameter" /> 
        /// object is located.
        /// </param>
        /// <param name="value">
        /// The new <see cref="T:System.Data.Common.DbParameter" /> value.
        /// </param>
        protected override void SetParameter(int index, DbParameter value)
        {
            _list[index] = (MockParameter)value;
        }


        /// <summary>
        /// Sets the <see cref="T:System.Data.Common.DbParameter" /> object with the 
        /// specified name to a new value.
        /// </summary>
        /// <param name="parameterName">
        /// The name of the <see cref="T:System.Data.Common.DbParameter" /> object in 
        /// the collection.
        /// </param>
        /// <param name="value">
        /// The new <see cref="T:System.Data.Common.DbParameter" /> value.
        /// </param>
        protected override void SetParameter(string parameterName, DbParameter value)
        {
            int pos = IndexOf(parameterName);

            if (pos >= 0)
            {
                SetParameter(pos, value);
            }
        }


        /// <summary>
        /// Returns the <see cref="T:System.Data.Common.DbParameter" /> object at the 
        /// specified index in the collection.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Data.Common.DbParameter" /> object at the specified 
        /// index in the collection.
        /// </returns>
        /// <param name="index">
        /// The index of the <see cref="T:System.Data.Common.DbParameter" /> in the collection.
        /// </param>
        protected override DbParameter GetParameter(int index)
        {
            return _list[index];
        }


        /// <summary>
        /// Returns <see cref="T:System.Data.Common.DbParameter" /> the object with the specified name.
        /// </summary>
        /// <returns>
        /// The <see cref="T:System.Data.Common.DbParameter" /> the object with the specified name.
        /// </returns>
        /// <param name="parameterName">
        /// The name of the <see cref="T:System.Data.Common.DbParameter" /> in the collection.
        /// </param>
        protected override DbParameter GetParameter(string parameterName)
        {
            int pos = IndexOf(parameterName);

            return (pos >= 0) ? _list[pos] : null;
        }
    }
}
