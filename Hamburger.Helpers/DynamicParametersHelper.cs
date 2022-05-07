using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Hamburger.Helpers
{
    public static class DynamicParametersHelper
    {
        /// <summary>
        /// Build dynamic object from object.
        /// </summary>
        /// <param name="obj">A non-primitive type object.</param>
        /// <returns>Instance of DynamicParameters.</returns>
        public static DynamicParameters BuildDynamicParameters(object obj)
        {
            var param = new DynamicParameters();

            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                object value = prop.GetValue(obj);
                param.Add(name: prop.Name, value: value, direction: ParameterDirection.Input);
            }

            return param;
        }

        /// <summary>
        /// Build dynamic object from list of keys and list of value
        /// </summary>
        /// <param name="keys">List of keys.</param>
        /// <param name="values">List of values.</param>
        /// <returns>Instance of DynamicParameters.</returns>
        public static DynamicParameters BuildDynamicParameters(IEnumerable<string> keys, IEnumerable<object> values)
        {
            if (keys.Count() > values.Count())
                throw CustomException.Database.NotProvidedEnoughValues;

            var keyList = keys.ToList();
            var valuesList = values.ToList();

            var param = new DynamicParameters();
            for (int i = 0; i < keyList.Count; i++)
            {
                param.Add(name: keyList[i], valuesList[i], direction: ParameterDirection.Input);
            }

            return param;
        }
    }
}
