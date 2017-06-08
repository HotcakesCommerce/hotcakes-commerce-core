using System.Collections.Generic;
using System.Linq;

namespace BrandonHaynes.ModelAdapter.EntityFramework
	{
	static class Extensions
		{
		/// <summary>
		/// Converts a singleton T to an enumerable set
		/// </summary>
		/// <typeparam name="T">The singleton's type</typeparam>
		/// <param name="singleton">The singleton</param>
		/// <returns>An IEnumerable<T> containing only the passed-in singleton</returns>
		public static IEnumerable<T> ToEnumerable<T>(this T singleton)
			{ return Enumerable.Repeat(singleton, 1); }

		/// <summary>
		/// Given a value, returns the substring occurring before the parameter search; string.Empty otherwise 
		/// </summary>
		/// <param name="value">The value to search</param>
		/// <param name="search">The string to search for</param>
		/// <returns>The substring occurring before search in the string value</returns>
		public static string SubstringBefore(this string value, char search)
			{
			var index = value.IndexOf(search);
			return index != -1 ? value.Substring(0, index) : string.Empty;
			}

		/// <summary>
		/// Given a value, returns the substring occurring after the parameter search; string.Empty otherwise
		/// </summary>
		/// <param name="value">The value to search</param>
		/// <param name="search">The string to search for</param>
		/// <returns>The substring occurring after search in the string value</returns>
		public static string SubstringAfter(this string value, char search)
			{
			var index = value.IndexOf(search);
			return index != -1 ? value.Substring(index + 1) : string.Empty;
			}
		}
	}
