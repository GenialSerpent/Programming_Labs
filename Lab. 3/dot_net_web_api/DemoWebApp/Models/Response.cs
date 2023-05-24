using System.Collections.Generic;

public class Response
{
    public int Status { get; set; }
    public Dictionary<string, List<string>> Errors { get; set; }
    public dynamic Data { get; set; }
}