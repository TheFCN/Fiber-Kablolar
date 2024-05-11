namespace TTNETFİBER;

public class ResultModel
{
    public string MaxSpeed { get; set; }
    public string SVUID { get; set; }
    public ADSL ADSL { get; set; }
    public VDSL VDSL { get; set; }
    public Fiber Fiber { get; set; }
}

public class ADSL
{
    public string PortState { get; set; }
    public string Distance { get; set; }
}

public class VDSL
{
    public string PortState { get; set; }
    public string Distance { get; set; }
}

public class Fiber
{
    public string PortState { get; set; }
    public string Distance { get; set; }
}