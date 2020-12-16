using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class Failable<T>
    {
        public readonly Exception Exception;
        public readonly T Result;
        public bool Failed => Exception != null;
        public Failable(Func<T> operation)
        {
            try
            {
                Result = operation();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        private Failable(T result, Exception exception)
        {
            Result = result;
            Exception = exception;
        }

        public Failable<U> Cast<U>()
        {
            return new Failable<U>((U)(object)Result, Exception);
        }
    }
}
