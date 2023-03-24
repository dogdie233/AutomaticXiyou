namespace XiyouApi
{
    public sealed class OperationResult
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSucceed { get; set; }
        
        public OperationResult(string message, bool isSucceed)
        {
            Message = message;
            IsSucceed = isSucceed;
        }
    }
}
