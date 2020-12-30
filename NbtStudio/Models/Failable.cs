using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public class Failable<T>
    {
        public readonly Exception Exception;
        private readonly T _Result;
        public T Result
        {
            get
            {
                if (Failed)
                    throw Exception;
                return _Result;
            }
        }
        public readonly string Description;
        private readonly List<Failable<T>> Nested;
        public ReadOnlyCollection<Failable<T>> SubFailures => Nested.AsReadOnly();
        public bool Failed => Exception != null;
        public bool IsAggregate => Nested.Any();
        public Failable(Func<T> operation) : this(operation, null) { }

        public Failable(Func<T> operation, string description)
        {
            Description = description;
            Nested = new List<Failable<T>>();
            try
            {
                _Result = operation();
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
        }

        public static Failable<T> Aggregate(params Failable<T>[] failures)
        {
            var flattened = failures.SelectMany(x => x.GetRelevantFailures()).ToList();
            var exception = new AggregateException(flattened.Select(x => x.Exception));
            string description = String.Join("\n", flattened.Select(x => x.Description));
            return new Failable<T>(default, exception, description, flattened);
        }

        private IEnumerable<Failable<T>> GetRelevantFailures()
        {
            if (IsAggregate)
                return Nested;
            return new[] { this };
        }

        private Failable(T result, Exception exception, string description, List<Failable<T>> subfailures = null)
        {
            _Result = result;
            Exception = exception;
            Description = description;
            Nested = subfailures ?? new List<Failable<T>>();
        }

        public Failable<U> Cast<U>()
        {
            return new Failable<U>((U)(object)_Result, Exception, Description, Nested.Select(x => x.Cast<U>()).ToList());
        }
    }
}
