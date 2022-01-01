namespace ValueOrErrorNS
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

        public TError Error => _error!;
        public TValue Value => _value!;

        public static ValueOrError<TValue, TError> WithError(TError error)
        {
            return new(
                isValue: false,
                error: error,
                value: default
            );
        }

        public static ValueOrError<TValue, TError> WithValue(TValue value)
        {
            return new(
                isValue: true,
                error: default,
                value: value
            );
        }
    }
}
