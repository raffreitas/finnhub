namespace FinnHub.PortfolioManagement.Application.Shared.Result;

public class Result
{
    protected Result(bool isSuccess, IEnumerable<string>? errors = null)
    {
        if (isSuccess && errors != null && errors.Any())
            throw new InvalidOperationException("A successful result cannot contain errors.");

        IsSuccess = isSuccess;
        Errors = errors?.ToList() ?? [];
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<string> Errors { get; }

    public static Result Success() => new(true);

    public static Result Failure(IEnumerable<string> errors) => new(false, errors);

    public static Result Failure(string error) => new(false, new[] { error });

    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    public static Result<T> Failure<T>(IEnumerable<string> errors) => Result<T>.Failure(errors);

    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}

public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T value, bool isSuccess, IEnumerable<string>? errors = null)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    protected internal Result(bool isSuccess, IEnumerable<string>? errors = null)
        : base(isSuccess, errors)
    {
        _value = default;
    }

    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access the value of a failed result. Use IsSuccess to check first.");

            return _value!;
        }
    }

    public static Result<T> Success(T value) => new(value, true);

    public static new Result<T> Failure(IEnumerable<string> errors) => new(false, errors);

    public static new Result<T> Failure(string error) => new(false, [error]);
}