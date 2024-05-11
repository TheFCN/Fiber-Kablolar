namespace TTNETFİBER;

public class ResponseModel
{
    public object InternalException { get; set; }
    public Data[] Data { get; set; }
}

public class Data
{
    public string Code { get; set; }
    public string Name { get; set; }
}

