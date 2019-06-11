using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bunifu.Data.Helper
{
    public static class BunifuMapper
    {

        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="datatable">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> MapToList<T>(DataTable datatable) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in datatable.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {

                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static List<T> MapToList<T>(DataView dataview) where T : class, new()
        {
            return MapToList<T>(dataview.ToTable());
        }

        public static string GenerateInsert<T>(T Object,string tableName) where T : class, new()
        {
            string cols = "(";
            string vals = "(";
            int i = 0;
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.GetValue(Object, null).ToString().Length>0 && i>0)
                {
                    cols += "`" + property.Name + "`,";
                    vals += "'" + property.GetValue(Object, null) + "',";
                }
                i++;
            }
            vals = vals.Substring(0, vals.Length - 1) + ")";
            cols = cols.Substring(0, cols.Length - 1) + ")";

            string str = "INSERT INTO `" + tableName + "` SET "+cols+" "+vals+";";

            return str;
        }
        public static string GenerateUpdate<T>(T Object, string tableName) where T : class, new()
        {
            string str = "UPDATE `" + tableName + "` SET ";
            int i = 0;
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (property.GetValue(Object, null).ToString().Length > 0 && i > 0)
                {
                    str += "`" + property.Name + "`='" + property.GetValue(Object, null) + "',";
                }
                i++;
            }
            str = str.Substring(0, str.Length - 1)+" WHERE `"+ typeof(T).GetProperties()[0].Name+"` = '"+ typeof(T).GetProperties()[0].GetValue(Object,null) + "';";
            return str;
        }

        public static string GenerateDelete<T>(T Object, string tableName) where T : class, new()
        {
           return "DELETE FROM `" + tableName + "` WHERE `" + typeof(T).GetProperties()[0].Name + "` = '" + typeof(T).GetProperties()[0].GetValue(Object, null) + "';";
            
        }

    }
}
