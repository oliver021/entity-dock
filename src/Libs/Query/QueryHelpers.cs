using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EntityDock.Extensions.Query
{

    /// <summary>
    /// Simple query helpers to make tasks
    /// </summary>
    public static class QueryHelpers
    {
        /// <summary>
        /// Simple helper extensions to get mappeable properties
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPropertiesMapped(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetProperties()
                .Where(x => x.CanWrite && x.CanRead)
                .Where(x => !x.IsDefined(typeof(NotMappedAttribute)));
        }

        /// <summary>
        /// Count All text
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<TextCount> CountAllText(List<object> data)
        {
            var result = new Dictionary<int, TextCount>();

            foreach (var item in data)
            {
                foreach (var p in item.GetType().GetPropertiesMapped()
                    .Where(x => x.PropertyType.IsEquivalentTo(typeof(string))))
                {
                    var current = p.GetValue(item);

                    // check if text is register and send
                    CheckText(result, current, p.Name);
                }
            }

            return result.Values;

            static void CheckText(Dictionary<int, TextCount> result, object current, string field)
            {
                if (current != null)
                {
                    int code = current.ToString().GetHashCode();

                    if (result.ContainsKey(code))
                    {
                        result[code].Increment();
                        result[code].RegisterField(field);
                    }
                    else
                    {
                        result[code] = new TextCount(current.ToString(), field);
                    }
                }
            }
        }
    }
}
