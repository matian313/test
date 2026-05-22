namespace Com.AIServe.Handlers.Setup.Models;

public class SystemConfig
{
    public string SystemName { get; set; } = "AI预约服务系统";
    public string Version { get; set; } = "1.0.0";
    public bool IsMaintenance { get; set; } = false;
}
