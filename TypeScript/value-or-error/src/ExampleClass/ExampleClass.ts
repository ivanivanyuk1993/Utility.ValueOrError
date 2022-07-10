/**
 * `ValueOrError` is needed to forcibly prevent accessing nullable values
 * without null check
 *
 * `ValueOrError` represents result of computation which can have either result,
 * or error(unlike some observable results, which can allow both latest known actual value, and latest error list,
 * like when error was caused by 0.01% downtime of server, and we can't know for sure if value is still actual, and
 * we don't need strong consistency, but need high availability, so it is ok to show both stale value and latest error(-s))
 *
 * todo
 */
export class ValueOrError<TValue, TError> {
  private readonly _error?: TError;
  private readonly _value?: TValue;
  public readonly isValue: boolean;
  
  private constructor(
        isValue: boolean,
        error?: TError,
        value?: TValue
    )
    {
        this.isValue = isValue;
      this._error = error;
      this._value = value;
    }
}
