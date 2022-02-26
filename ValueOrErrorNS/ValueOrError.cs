﻿namespace ValueOrErrorNS
{
    /// <summary>
    ///     todo optimize properties(consider storing as single object property and casting and making not struct, but class),
    ///     knowing that we will usually read error/value once, pass between services at most some times(structs are passed by copy),
    ///     and construct only once
    /// </summary>
    public class ValueOrError<TValue, TError>
    {
        public readonly bool IsValue;

        private readonly TError? _error;
        private readonly TValue? _value;

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

        public static ValueOrError<TValue, TError> Error(TError error)
        {
            return new(
                isValue: false,
                error: error,
                value: default
            );
        }

        public static ValueOrError<TValue, TError> Value(TValue value)
        {
            return new(
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
            {
                runActionWithValueFunc(obj: _value!);
            }
            else
            {
                runActionWithErrorFunc(obj: _error!);
            }
        }

        public ValueOrError<TResultWithValue, TResultWithError> RunActionWithResultWithValueOrError<TResultWithValue, TResultWithError>(
            Func<TValue, TResultWithValue> runActionWithResultWithValueFunc,
            Func<TError, TResultWithError> runActionWithResultWithErrorFunc
        )
        {
            return IsValue
                ? ValueOrError<TResultWithValue, TResultWithError>.Value(
                    value: runActionWithResultWithValueFunc(
                        arg: _value!
                    )
                )
                : ValueOrError<TResultWithValue, TResultWithError>.Error(
                    error: runActionWithResultWithErrorFunc(
                        arg: _error!
                    )
                );
        }

        public Task RunAsyncActionWithValueOrError(
            Func<TValue, Task> runAsyncActionWithValueFunc,
            Func<TError, Task> runAsyncActionWithErrorFunc
        )
        {
            return IsValue
                ? runAsyncActionWithValueFunc(arg: _value!)
                : runAsyncActionWithErrorFunc(arg: _error!);
        }

        public ValueOrError<Task<TResultWithValue>, Task<TResultWithError>> RunAsyncActionWithResultWithValueOrError<TResultWithValue, TResultWithError>(
            Func<TValue, Task<TResultWithValue>> runAsyncActionWithResultWithValueFunc,
            Func<TError, Task<TResultWithError>> runAsyncActionWithResultWithErrorFunc
        )
        {
            return IsValue
                ? ValueOrError<Task<TResultWithValue>, Task<TResultWithError>>.Value(
                    value: runAsyncActionWithResultWithValueFunc(
                        arg: _value!
                    )
                )
                : ValueOrError<Task<TResultWithValue>, Task<TResultWithError>>.Error(
                    error: runAsyncActionWithResultWithErrorFunc(
                        arg: _error!
                    )
                );
        }
    }
}
