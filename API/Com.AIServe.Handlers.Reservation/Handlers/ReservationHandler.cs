using Com.AIServe.Common.Data;
using Com.AIServe.Common.Handlers;
using Com.AIServe.Common.Models;
using Com.AIServe.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ReservationModel = Com.AIServe.Common.Models.Reservation;

namespace Com.AIServe.Handlers.Reservation.Handlers;

public class ReservationHandler : IHandler
{
    private static readonly System.Text.Json.JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
    };

    private readonly AppDbContext _db;

    public ReservationHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ApiResponse> HandleRequestAsync(HttpContext context, IQueryCollection query, string body)
    {
        var action = query["action"].ToString();
        LogHelper.Info($"ReservationHandler.HandleRequest - action: {action}");

        return action switch
        {
            "reservation_list" => await GetListAsync(),
            "reservation_get" => await GetAsync(query),
            "reservation_save" => await SaveAsync(body),
            "reservation_create" => await SaveAsync(body),
            "reservation_update" => await SaveAsync(body),
            "reservation_updatestatus" => await UpdateStatusAsync(body),
            "reservation_delete" => await DeleteAsync(query),
            _ => ApiResponse.Fail("无效的请求"),
        };
    }

    private async Task<ApiResponse> GetListAsync()
    {
        LogHelper.Info("获取在线预约");
        var list = await _db.Reservations.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return ApiResponse.Ok(list);
    }

    private async Task<ApiResponse> GetAsync(IQueryCollection query)
    {
        if (!int.TryParse(query["id"], out int id))
        {
            return ApiResponse.Fail("无效的id");
        }

        LogHelper.Info($"获取预约详情: {id}");
        var reservation = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        return reservation != null ? ApiResponse.Ok(reservation) : ApiResponse.Fail("预约不存在");
    }

    private async Task<ApiResponse> SaveAsync(string body)
    {
        try
        {
            var reservation = JsonSerializer.Deserialize<ReservationModel>(body, _jsonOptions);
            if (reservation == null)
            {
                return ApiResponse.Fail("无效的请求数据");
            }

            var isCreate = reservation.Id <= 0;

            if (isCreate)
            {
                LogHelper.Info("创建预约");
                reservation.CreatedAt = DateTime.Now;
                reservation.Status = 1;
                _db.Reservations.Add(reservation);
            }
            else
            {
                LogHelper.Info($"更新预约: {reservation.Id}");
                var existing = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == reservation.Id);
                if (existing == null) return ApiResponse.Fail("预约不存在");

                existing.CustomerName = reservation.CustomerName;
                existing.Phone = reservation.Phone;
                existing.ReservationTime = reservation.ReservationTime;
                existing.ServiceType = reservation.ServiceType;
                existing.Remark = reservation.Remark;
                existing.UpdatedAt = DateTime.Now;
            }

            await _db.SaveChangesAsync();
            return ApiResponse.Ok();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"保存预约失败: {ex.Message}");
        }
    }

    private async Task<ApiResponse> UpdateStatusAsync(string body)
    {
        try
        {
            var request = JsonSerializer.Deserialize<UpdateStatusRequest>(body, _jsonOptions);
            if (request == null)
            {
                return ApiResponse.Fail("无效的请求数据");
            }

            LogHelper.Info($"更新预约状态: {request.Id} -> {request.Status}");
            var existing = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == request.Id);
            if (existing == null) return ApiResponse.Fail("预约不存在");

            existing.Status = request.Status;
            existing.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();
            return ApiResponse.Ok();
        }
        catch (Exception ex)
        {
            return ApiResponse.Fail($"更新预约状态失败: {ex.Message}");
        }
    }

    private async Task<ApiResponse> DeleteAsync(IQueryCollection query)
    {
        if (!int.TryParse(query["id"], out int id))
        {
            return ApiResponse.Fail("无效的id");
        }

        LogHelper.Info($"删除预约: {id}");
        var existing = await _db.Reservations.FirstOrDefaultAsync(r => r.Id == id);
        if (existing == null) return ApiResponse.Fail("预约不存在");

        _db.Reservations.Remove(existing);
        await _db.SaveChangesAsync();
        return ApiResponse.Ok();
    }
}

public class UpdateStatusRequest
{
    public int Id { get; set; }
    public int Status { get; set; }
}
