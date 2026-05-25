using Com.AIServe.Common.Handlers;
using Com.AIServe.Common.Models;
using Com.AIServe.Handlers.Setup.Models;
using Com.AIServe.Utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Com.AIServe.Handlers.Setup.Handlers;

[Handler("report")]
public class ReportHandler : IHandler
{
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    public async Task<ApiResponse> HandleRequestAsync(HttpContext context, IQueryCollection query, string body)
    {
        var action = query["action"].ToString();
        LogHelper.Info($"ReportHandler.HandleRequest - action: {action}");

        return action switch
        {
            "report_getdata" => await Task.FromResult(GetReportData(query)),
            _ => await Task.FromResult(ApiResponse.Fail("无效的请求")),
        };
    }

    private ApiResponse GetReportData(IQueryCollection query)
    {
        var timeType = query["timeType"].ToString();
        var date = query["date"].ToString();
        LogHelper.Info($"获取报表数据: timeType={timeType}, date={date}");

        var reportData = new ReportData
        {
            CoreIndicators = new CoreIndicators
            {
                TotalCustomers = 580,
                NewCustomers = 80,
                RetentionRate = 68.5
            },
            ChannelSources = new List<ChannelSource>
            {
                new() { Value = 40, Name = "预订电话", Color = "#1e40af" },
                new() { Value = 120, Name = "活码", Color = "#3b82f6" },
                new() { Value = 60, Name = "主动添加", Color = "#93c5fd" },
                new() { Value = 40, Name = "桌台码", Color = "#10b981" }
            },
            ReservationTrend = new ReservationTrend
            {
                Dates = new List<string> { "1日", "6日", "11日", "16日", "21日", "26日", "31日" },
                AiWechatReservations = new List<int> { 110, 125, 130, 110, 115, 145, 120 },
                AiPhoneReservations = new List<int> { 90, 85, 70, 90, 30, 40, 35 }
            }
        };

        return ApiResponse.Ok(reportData);
    }
}
