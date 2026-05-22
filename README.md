# AIReservationServe - 简化版

AI预约服务系统简化版项目

## 项目结构

```
AIServe.sln
├── API/
│   ├── AIServe.API/                    # 主API项目
│   │   ├── Controllers/
│   │   │   ├── ReservationController.cs
│   │   │   └── SetupController.cs
│   │   └── Program.cs
│   ├── Com.AIServe.Common/             # 公共类库
│   │   └── Models/
│   │       ├── Reservation.cs
│   │       └── ApiResponse.cs
│   ├── Com.AIServe.Utils/              # 工具类库
│   │   ├── JsonHelper.cs
│   │   └── LogHelper.cs
│   ├── Com.AIServe.Handlers.Reservation/  # 预约处理模块
│   │   └── ReservationHandler.cs
│   └── Com.AIServe.Handlers.Setup/        # 系统设置模块
│       └── SetupHandler.cs
├── doc/                                # 文档目录
├── log/                                # 日志目录
└── Reservation/                        # 预约相关
```

## 运行项目

```bash
cd API/AIServe.API
dotnet run
```

访问 Swagger 文档: http://localhost:5000/swagger

## API 接口

### 预约管理
- `GET /api/reservation` - 获取预约列表
- `GET /api/reservation/{id}` - 获取预约详情
- `POST /api/reservation` - 创建预约
- `PUT /api/reservation/{id}` - 更新预约
- `DELETE /api/reservation/{id}` - 删除预约
- `PATCH /api/reservation/{id}/status` - 更新预约状态

### 系统设置
- `GET /api/setup/config` - 获取系统配置
- `POST /api/setup/config` - 更新系统配置
- `GET /api/setup/health` - 健康检查
