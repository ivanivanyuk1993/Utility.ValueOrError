using System.Reactive;
using System.Reactive.Linq;

namespace ValueOrErrorNS;

/// <summary>
///     <para>
///         <see cref="ValueOrError{TValue,TError}" /> is needed to forcibly prevent accessing nullable values without
///         null check
///     </para>
///     <para>
///         <see cref="ValueOrError{TValue,TError}" /> represents result of computation which can have either result,
///         or error(unlike some observable results, which can allow both latest known actual value, and latest error list,
///         like when error was caused by 0.01% downtime of server, and we can't know for sure if value is still actual,
///         and
///         we don't need strong consistency, but need high availability, so it is ok to show both stale value and latest
///         error(-s))
///     </para>
///     <para>
///         We use <see cref="IObservable{T}" />-s instead of <see cref="Task" />-s, because library provides
///         implementations in multiple languages, and Task/Promise/Future differ too much, and have problems, which
///         <see cref="IObservable{T}" />-s don't have, like
///         - C# `Task` is difficult to use with something like `IScheduler`-s
///         - JavaScript `Promise` doesn't support cancellation or something like `IScheduler`-s
///         - Java `Future` supports cancellation not through <see cref="CancellationToken"/>-s, and it looks like it
///         doesn't provide a way to cancel already running logic
///     </para>
/// </summary>
public class ValueOrError<TValue, TError>
{
    private readonly TError? _error;
    private readonly TValue? _value;
    public readonly bool IsValue;

    private ValueOrError(
        bool isValue,
        TError? error,
        TValue? value
    )
    {
        IsValue = isValue;
        _error = error;
        _value = value;
    }

    public static ValueOrError<TValue, TError> CreateError(TError error)
    {
        return new ValueOrError<TValue, TError>(
            isValue: false,
            error: error,
            value: default
        );
    }

    public static ValueOrError<TValue, TError> CreateValue(TValue value)
    {
        return new ValueOrError<TValue, TError>(
            isValue: true,
            error: default,
            value: value
        );
    }

    public void RunActionWithValueOrError(
        Action<TValue> runActionWithValueFunc,
        Action<TError> runActionWithErrorFunc
    )
    {
        if (IsValue)
            runActionWithValueFunc(obj: _value!);
        else
            runActionWithErrorFunc(obj: _error!);
    }

    public ValueOrError<
        TResultWithValue,
        TResultWithError
    > RunActionWithResultWithValueOrError<TResultWithValue, TResultWithError>(
        Func<TValue, TResultWithValue> runActionWithResultWithValueFunc,
        Func<TError, TResultWithError> runActionWithResultWithErrorFunc
    )
    {
        return IsValue
            ? ValueOrError<TResultWithValue, TResultWithError>.CreateValue(
                value: runActionWithResultWithValueFunc(
                    arg: _value!
                )
            )
            : ValueOrError<TResultWithValue, TResultWithError>.CreateError(
                error: runActionWithResultWithErrorFunc(
                    arg: _error!
                )
            );
    }

    public IObservable<Unit> RunActionWithValueOrErrorReactive(
        Func<TValue, IObservable<Unit>> runActionWithValueReactiveFunc,
        Func<TError, IObservable<Unit>> runActionWithErrorReactiveFunc
    )
    {
        // We can use `IsValue` directly(not in `Observable.Defer`), because it is `readonly`
        return IsValue
            ? runActionWithValueReactiveFunc(arg: _value!)
            : runActionWithErrorReactiveFunc(arg: _error!);
    }

    public IObservable<ValueOrError<TResultWithValue, TResultWithError>> RunActionWithResultWithValueOrErrorReactive<
        TResultWithValue,
        TResultWithError
    >(
        Func<TValue, IObservable<TResultWithValue>> runActionWithResultWithValueReactiveFunc,
        Func<TError, IObservable<TResultWithError>> runActionWithResultWithErrorReactiveFunc
    )
    {
        // We can use `IsValue` directly(not in `Observable.Defer`), because it is `readonly`
        return IsValue
            ? runActionWithResultWithValueReactiveFunc(arg: _value!)
                .Select(selector: ValueOrError<TResultWithValue, TResultWithError>.CreateValue)
            : runActionWithResultWithErrorReactiveFunc(arg: _error!)
                .Select(selector: ValueOrError<TResultWithValue, TResultWithError>.CreateError);
    }
}
