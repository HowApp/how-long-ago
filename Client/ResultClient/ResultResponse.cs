namespace How.Client.ResultClient;

public class ResultResponse
{
    public bool Succeed { get; set; }
    public bool Failed { get; set; }
    public Dictionary<string, string> Error { get; set; }
}

public class ResultResponse<TData> : ResultResponse
{
    public TData? Data { get; set; }
}