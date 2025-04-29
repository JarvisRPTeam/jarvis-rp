#nullable enable

namespace GameDb.Repository {
    public class DbQueryResult<TEntity> where TEntity : class {
        public DbResultType ResultType { get; set; }
        public TEntity? ReturnValue { get; set; }
        public string Message { get; set; } 

        public DbQueryResult(DbResultType resultType, string message, TEntity? returnValue = null) {
            ResultType = resultType;
            Message = message;
            ReturnValue = returnValue;
        }
    }

    public enum DbResultType {
        Success,
        Error,
        Warning
    }
}