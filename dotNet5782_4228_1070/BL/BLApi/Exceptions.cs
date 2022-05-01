using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BO
{
    public partial class Exceptions
    {


        public class ObjNotExistException : Exception
        {
            public ObjNotExistException(Type t, int id, Exception exception)
                : base(String.Format($"The {t.Name} with id {id} doesn't exist."), exception)
            {
            }
            public ObjNotExistException(Type t, int id)
                : base(String.Format($"The {t.Name} with id {id} doesn't exist."))
            {
            }

            public ObjNotExistException(string messageFromDal)
                : base(String.Format($"{messageFromDal}"))
            {
            }
        }

        public class DataChanged : Exception
        {
            public DataChanged(Type objType, int id, string message)
                : base(String.Format($"The {objType.Name} with id: {id} : Data Changed\n{message}."))
            {
            }
        }

        public class ObjExistException : Exception
        {
            public ObjExistException(Type objType, int id)
                : base(String.Format($"The {objType.Name} with id: {id} exist."))
            {
            }
        }

        public class NoDataMatchingBetweenDalandBL : Exception
        {
            public NoDataMatchingBetweenDalandBL(string message)
                : base(String.Format($"{message}"))
            {
            }
        }

        public class ObjNotAvailableException : Exception
        {
            public ObjNotAvailableException(string message, Exception exception)
                : base(string.Format($"ERROR: {message}"), exception)
            {
            }
            public ObjNotAvailableException(string message)
                : base(string.Format($"ERROR: {message}"))
            {
            }
            public ObjNotAvailableException(Type objType, int id, string message)
                : base(string.Format($"ERROR: The {objType} with id: {id} \n{message}"))
            {
            }

        }

        public class FileLoadCreateException : Exception
        {
            public FileLoadCreateException(string message, Exception exception)
                : base(string.Format($"{message}"), exception) { }
        }
    }
}
