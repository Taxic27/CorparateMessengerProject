namespace Server.Tools
{
    public class Result
    {
        public bool IsSuccess => Errors.Length == 0;
        public string[] Errors { get; }
        public string ErrorsAsString => string.Join(", ", Errors);

        public Result(string[] errors)
        {
            Errors = errors;
        }

        public static Result Success()
        {
            return new Result([]);
        }

        public static Result Fail(string error)
        {
            return new Result([error]);
        }

        public static Result Fail(IEnumerable<string> error)
        {
            return new Result(error.ToArray());
        }
    }
}
