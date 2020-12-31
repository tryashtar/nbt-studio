using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbtStudio
{
    public abstract class Failable
    {
        public abstract string ToStringSimple();
        public abstract string ToStringDetailed();
    }

    public class Failable<T> : Failable
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

        public static Failable<T> Failure(Exception exc, string description)
        {
            return new Failable<T>(default, exc, description);
        }

        public static Failable<T> AggregateFailure(params Exception[] exceptions)
        {
            return Aggregate(exceptions.Select(x => Failure(x, null)).ToArray());
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

        public override string ToStringSimple()
        {
            if (IsAggregate)
            {
                var messages = new HashSet<string>();
                var summaries = new List<string>();
                foreach (var item in Nested)
                {
                    if (messages.Add(item.Exception.Message))
                        summaries.Add(item.ToStringSimple());
                }
                return String.Join("\n", summaries);
            }
            else
            {
                if (Failed)
                    return Util.ExceptionMessage(Exception);
                else
                    return $"{Description}: Operation succeeded";
            }
        }

        public override string ToStringDetailed()
        {
            if (IsAggregate)
                return String.Join("\n\n", Nested.Select(x => x.ToStringDetailed()));
            else
            {
                if (Failed)
                    return $"{Description}:\n{Exception}";
                else
                    return $"{Description}: Operation succeeded";
            }
        }
    }
}
