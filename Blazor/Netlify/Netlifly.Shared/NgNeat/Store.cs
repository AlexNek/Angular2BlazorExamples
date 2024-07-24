using System.Reactive.Subjects;

namespace Netlifly.Shared.MyNamespace;

//public class Store
//{
//    // Add implementation for Store if needed
//}

// C# code translated from TypeScript

using System;
using System.Collections.Generic;

//public class Store<SDef, State> : BehaviorSubject<State>
//{
//    private SDef storeDef;
//    private State initialState;
//    private State state;
//    private bool batchInProgress;
//    private object context;

//    public Store(SDef storeDef)
//    {
//        this.storeDef = storeDef;
//    }

//    public SDef Name
//    {
//        get { return storeDef.name; }
//    }

//    private State GetInitialState()
//    {
//        // Implement getInitialState logic here
//        return default(State);
//    }

//    public Config GetConfig<Config>() where Config : Dictionary<object, object>
//    {
//        // Implement getConfig logic here
//        return default(Config);
//    }

//    public R Query<R>(Query<State, R> selector)
//    {
//        // Implement query logic here
//        return default(R);
//    }

//    public void Update(params Reducer<State>[] reducers)
//    {
//        // Implement update logic here
//    }

//    public State GetValue()
//    {
//        return state;
//    }

//    public void Reset()
//    {
//        // Implement reset logic here
//    }

//    public IObservable<Dictionary<string, object>> Combine<O>(O observables) where O : Dictionary<string, IObservable<object>>
//    {
//        // Implement combine logic here
//        return null;
//    }

//    public void Destroy()
//    {
//        // Implement destroy logic here
//    }

//    public void Next(State value)
//    {
//        // Implement next logic here
//    }

//    public void Error()
//    {
//        // Implement error logic here
//    }

//    public void Complete()
//    {
//        // Implement complete logic here
//    }
//}

//public class StoreValue<T> where T : Store
//{
//    public Type Value { get; set; }
//}

public delegate State Reducer<State>(State state, ReducerContext context);

public class ReducerContext
{
    public Dictionary<object, object> Config { get; set; }
}

//public interface StoreDef<State>
//{
//    string Name { get; set; }
//    State State { get; set; }
//    object Config { get; set; }
//}