
using System.Collections.Generic;
using System;
using System.Linq;

namespace NLP
{
    public static class Extensions
    {
        public static bool CaseInsensitiveContains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Determines whether the collection is null or contains no elements.
        /// </summary>
        /// <typeparam name="T">The IEnumerable type.</typeparam>
        /// <param name="enumerable">The enumerable, which may be null or empty.</param>
        /// <returns>
        ///     <c>true</c> if the IEnumerable is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }
            /* If this is a list, use the Count property for efficiency. 
             * The Count property is O(1) while IEnumerable.Count() is O(N). */
            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    [Serializable]
    public class MappingAttribute : Attribute
    {
        public string ColumnName = null;
    }
    public class ReplaceData
    {
        [Mapping(ColumnName ="Row")]
        public int Row { get; set; }
        [Mapping(ColumnName = "Email")]
        public string Email { get; set; }
        [Mapping(ColumnName = "Phone")]
        public string Phone { get; set; }
        [Mapping(ColumnName = "Address")]
        public string Address { get; set; }
    }


    public class Inbox
    {
        [Mapping(ColumnName = "Id")]
        public int Id { get; set; }
        [Mapping(ColumnName = "FromPerson")]
        public string FromPerson { get; set; }
        [Mapping(ColumnName = "Subject")]
        public string Subject { get; set; }
        [Mapping(ColumnName = "Body")]
        public string Body { get; set; }
        [Mapping(ColumnName = "Redacted")]
        public string Redacted { get; set; }

    }

    public class csvData
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
     }

    public class csvDataRows
    {
        public List<csvData> Rows { get; private set; }
    }

}
 






