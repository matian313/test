namespace Com.AIServe.Handlers.Setup.Models;

public class ReportData
{
    public CoreIndicators CoreIndicators { get; set; } = new();
    public List<ChannelSource> ChannelSources { get; set; } = new();
    public ReservationTrend ReservationTrend { get; set; } = new();
}

public class CoreIndicators
{
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public double RetentionRate { get; set; }
}

public class ChannelSource
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class ReservationTrend
{
    public List<string> Dates { get; set; } = new();
    public List<int> AiWechatReservations { get; set; } = new();
    public List<int> AiPhoneReservations { get; set; } = new();
}
