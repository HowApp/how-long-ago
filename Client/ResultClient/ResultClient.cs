namespace How.Client.ResultClient;

public class ResultClient<Tdata>
{
    public Tdata? Data { get; set; }
    public bool Succeed { get; set; }
    public bool Failed { get; set; }
    public Dictionary<string, string> Error { get; set; }
}