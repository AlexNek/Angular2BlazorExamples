using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netlifly.Shared.NgNeat
{// Technology stack: C#

    using System;
    using System.Collections.Generic;
    

   
        /// <summary>
        /// A Subject is a special type of Observable that allows values to be
        /// multicasted to many Observers. Subjects are like EventEmitters.
        /// </summary>
        //public class Subject<T> : Observable<T>, ISubscriptionLike
        //{
        //    public bool Closed { get; set; }
        //    private List<IObserver<T>> CurrentObservers { get; set; }

        //    [Obsolete("Internal implementation detail, do not use directly. Will be made internal in v8.")]
        //    public List<IObserver<T>> Observers { get; set; }

        //    [Obsolete("Internal implementation detail, do not use directly. Will be made internal in v8.")]
        //    public bool IsStopped { get; set; }

        //    [Obsolete("Internal implementation detail, do not use directly. Will be made internal in v8.")]
        //    public bool HasError { get; set; }

        //    [Obsolete("Internal implementation detail, do not use directly. Will be made internal in v8.")]
        //    public object ThrownError { get; set; }

        //    [Obsolete("Recommended you do not use. Will be removed at some point in the future. Plans for replacement still under discussion.")]
        //    public static Func<object[], object> Create;

        //    public Subject()
        //    {
        //        CurrentObservers = new List<IObserver<T>>();
        //        Observers = new List<IObserver<T>>();
        //    }

        //    [Obsolete("Internal implementation detail, do not use directly. Will be made internal in v8.")]
        //    public Observable<R> Lift<R>(Operator<T, R> @operator)
        //    {
        //        // Implementation here
        //    }

        //    public void Next(T value)
        //    {
        //        // Implementation here
        //    }

        //    public void Error(object err)
        //    {
        //        // Implementation here
        //    }

        //    public void Complete()
        //    {
        //        // Implementation here
        //    }

        //    public void Unsubscribe()
        //    {
        //        // Implementation here
        //    }

        //    public bool Observed { get; }

        //    /// <summary>
        //    /// Creates a new Observable with this Subject as the source. You can do this
        //    /// to create custom Observer-side logic of the Subject and conceal it from
        //    /// code that uses the Observable.
        //    /// </summary>
        //    /// <returns>Observable that the Subject casts to</returns>
        //    public Observable<T> AsObservable()
        //    {
        //        // Implementation here
        //    }
        //}

        /// <summary>
        /// AnonymousSubject
        /// </summary>
        //public class AnonymousSubject<T> : Subject<T>
        //{
        //    [Obsolete("Internal implementation detail, do not use directly. Will be made internal in v8.")]
        //    public IObserver<T> Destination { get; set; }

        //    public AnonymousSubject(IObserver<T> destination = null, Observable<T> source = null)
        //    {
        //        Destination = destination;
        //    }

        //    public void Next(T value)
        //    {
        //        // Implementation here
        //    }

        //    public void Error(object err)
        //    {
        //        // Implementation here
        //    }

        //    public void Complete()
        //    {
        //        // Implementation here
        //    }
        //}
    }

public interface ISubscriptionLike : IUnsubscribable
{
    //void Unsubscribe();
    bool Closed { get; }
}

public interface IUnsubscribable
{
    void Unsubscribe();
}