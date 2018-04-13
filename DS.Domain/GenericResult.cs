namespace DS.Domain
{
    public class GenericResult
    {
        public GenericResult(bool ok, string message)
        {
            ExecutionOk = ok;
            Message = message;
        }
        public bool ExecutionOk { get; set; }
        public string Message { get; set; }
    }
}