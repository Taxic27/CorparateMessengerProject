namespace Server.Tools
{
    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(T? data, string[] errors) : base(errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(data, []);
        public new static Result<T> Fail(string error) => new(default, [error]);
    }
}
