using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    static class Cloning
    {
        public static T Clone<T>(this T original) where T : new()
        {
            T newObj = new T();
            foreach (PropertyInfo prop in typeof(T).GetProperties())
                prop.SetValue(newObj, prop.GetValue(original, null), null);
            return newObj;
        }
    }
}
