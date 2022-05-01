using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using DO;
using System.Runtime.CompilerServices;


namespace Dal
{

    public class XMLTools
    {
        #region SaveLoadWithXMLSerializer
        public static void SaveListToXMLSerializer<T>(IEnumerable<T> list, string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Create);
                XmlSerializer x = new XmlSerializer(list.GetType());
                x.Serialize(file, list);
                file.Close();
            }
            catch (Exception ex)
            {
                throw new Exceptions.FileLoadCreateException(filePath, $"fail to create xml file: {filePath}", ex);
            }
        }

        public static IEnumerable<T> LoadListFromXMLSerializer<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    IEnumerable<T> list;
                    XmlSerializer x = new XmlSerializer(typeof(List<T>));
                    FileStream file = new FileStream(filePath, FileMode.Open);
                    list = (IEnumerable<T>)x.Deserialize(file);
                    file.Close();
                    return list;
                }

            }
            catch (Exception ex)
            {
                throw new Exceptions.FileLoadCreateException(filePath, $"fail to create xml file: {filePath}", ex);
            }

            throw new Exceptions.ObjNotExistException($"{filePath}");

        }
        #endregion

        public static XElement LoadData(string filePath)
        {
            try
            {
                return XElement.Load(filePath);
            }
            catch
            {
                return null;
            }
        }
    }
}

