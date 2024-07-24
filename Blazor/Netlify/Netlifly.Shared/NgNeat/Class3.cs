using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Netlifly.Shared.NgNeat
{
    // C# code translation:

    // Technology stack: .NET Framework

    using System;
    using System.Threading.Tasks;

    //namespace RxLibrary
    //{
    //    public class Observable<T> : Subscribable<T>
    //    {
    //        [Obsolete]
    //        public Observable<object> Source { get; set; }

    //        [Obsolete]
    //        public Operator<object, T> Operator { get; set; }

    //        public Observable(Func<Observable<T>, Subscriber<T>, TeardownLogic> subscribe = null)
    //        {
    //            // Constructor implementation
    //        }

    //        public static Func<object[], object> Create { get; set; }

    //        public Observable<R> Lift<R>(Operator<T, R> @operator)
    //        {
    //            // Lift method implementation
    //        }

    //        public Subscription Subscribe(Partial<Observer<T>> observerOrNext = null, Action<T> next = null, Action<Exception> error = null, Action complete = null)
    //        {
    //            // Subscribe method implementation
    //        }

    //        public async Task ForEach(Action<T> next, PromiseConstructorLike promiseCtor = null)
    //        {
    //            // ForEach method implementation
    //        }

    //        public Observable<T> Pipe()
    //        {
    //            // Pipe method implementation
    //        }

    //        public Observable<A> Pipe<A>(OperatorFunction<T, A> op1)
    //        {
    //            // Overloaded Pipe method implementation
    //        }

    //        // More overloaded Pipe methods for various combinations of operators

    //        public Observable<unknown> Pipe<A, B, C, D, E, F, G, H, I>(OperatorFunction<T, A> op1, OperatorFunction<A, B> op2, OperatorFunction<B, C> op3, OperatorFunction<C, D> op4, OperatorFunction<D, E> op5, OperatorFunction<E, F> op6, OperatorFunction<F, G> op7, OperatorFunction<G, H> op8, OperatorFunction<H, I> op9, params OperatorFunction<object, object>[] operations)
    //        {
    //            // Pipe method implementation with multiple operators
    //        }
    //    }
    //}
}
/**
   * Used as a NON-CANCELLABLE means of subscribing to an observable, for use with
   * APIs that expect promises, like `async/await`. You cannot unsubscribe from this.
   *
   * **WARNING**: Only use this with observables you *know* will complete. If the source
   * observable does not complete, you will end up with a promise that is hung up, and
   * potentially all of the state of an async function hanging out in memory. To avoid
   * this situation, look into adding something like {@link timeout}, {@link take},
   * {@link takeWhile}, or {@link takeUntil} amongst others.
   *
   * #### Example
   *
   * ```ts
   * import { interval, take } from 'rxjs';
   *
   * const source$ = interval(1000).pipe(take(4));
   *
   * async function getTotal() {
   *   let total = 0;
   *
   *   await source$.forEach(value => {
   *     total += value;
   *     console.log('observable -> ' + value);
   *   });
   *
   *   return total;
   * }
   *
   * getTotal().then(
   *   total => console.log('Total: ' + total)
   * );
   *
   * // Expected:
   * // 'observable -> 0'
   * // 'observable -> 1'
   * // 'observable -> 2'
   * // 'observable -> 3'
   * // 'Total: 6'
   * ```
   *
   * @param next a handler for each value emitted by the observable
   * @return a promise that either resolves on observable completion or
   *  rejects with the handled error
   */

/**
     * @param next a handler for each value emitted by the observable
     * @param promiseCtor a constructor function used to instantiate the Promise
     * @return a promise that either resolves on observable completion or
     *  rejects with the handled error
     * @deprecated Passing a Promise constructor will no longer be available
     * in upcoming versions of RxJS. This is because it adds weight to the library, for very
     * little benefit. If you need this functionality, it is recommended that you either
     * polyfill Promise, or you create an adapter to convert the returned native promise
     * to whatever promise implementation you wanted. Will be removed in v8.
     */

/**
 * Creates a new Observable, with this Observable instance as the source, and the passed
 * operator defined as the new observable's operator.
 * @method lift
 * @param operator the operator defining the operation to take on the observable
 * @return a new observable with the Operator applied
 * @deprecated Internal implementation detail, do not use directly. Will be made internal in v8.
 * If you have implemented an operator using `lift`, it is recommended that you create an
 * operator by simply returning `new Observable()` directly. See "Creating new operators from
 * scratch" section here: https://rxjs.dev/guide/operators
 */

/**
     * Creates a new Observable by calling the Observable constructor
     * @owner Observable
     * @method create
     * @param {Function} subscribe? the subscriber function to be passed to the Observable constructor
     * @return {Observable} a new observable
     * @nocollapse
     * @deprecated Use `new Observable()` instead. Will be removed in v8.
     */

/**
 * A representation of any set of values over any amount of time. This is the most basic building block
 * of RxJS.
 *
 * @class Observable<T>
 */

// Internal implementation detail, do not use directly. Will be made internal in v8.

///** @deprecated Replaced with {@link firstValueFrom} and {@link lastValueFrom}. Will be removed in v8. Details: https://rxjs.dev/deprecations/to-promise */
//toPromise(): Promise < T | undefined >;
///** @deprecated Replaced with {@link firstValueFrom} and {@link lastValueFrom}. Will be removed in v8. Details: https://rxjs.dev/deprecations/to-promise */
//toPromise(PromiseCtor: typeof Promise): Promise < T | undefined >;
///** @deprecated Replaced with {@link firstValueFrom} and {@link lastValueFrom}. Will be removed in v8. Details: https://rxjs.dev/deprecations/to-promise */
//toPromise(PromiseCtor: PromiseConstructorLike): Promise < T | undefined >;