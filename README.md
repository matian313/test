# AIReservationServe - AI预约服务系统

AI预约服务系统，使用 .NET 10 + EF Core + SQL Server 开发

## 项目结构

```
AIServe.sln
├── API/
│   ├── AIServe.API/                    # 主API项目
│   │   ├── Controllers/
│   │   │   └── ApiServiceController.cs   # 统一API入口
│   │   ├── Handlers/
│   │   │   └── HandlerServiceExtensions.cs # Handler工厂和注册
│   │   ├── wwwroot/
│   │   │   ├── index.html              # 主页面
│   │   │   ├── login.html              # 登录页面
│   │   │   ├── css/style.css           # 样式
│   │   │   └── js/                     # JavaScript
│   │   ├── appsettings.json            # 配置文件
│   │   └── Program.cs
│   ├── Com.AIServe.Common/             # 公共类库
│   │   ├── Models/
│   │   │   ├── Reservation.cs
│   │   │   └── ApiResponse.cs
│   │   ├── Data/
│   │   │   └── AppDbContext.cs         # EF Core 数据库上下文
│   │   └── Handlers/
│   │       └── IHandler.cs
│   ├── Com.AIServe.Utils/              # 工具类库
│   │   └── LogHelper.cs
│   ├── Com.AIServe.Handlers.Reservation/  # 预约处理模块
│   │   └── ReservationHandler.cs
│   └── Com.AIServe.Handlers.Setup/        # 系统设置模块
│       ├── SetupHandler.cs
│       └── LoginHandler.cs
└── doc/                                # 文档目录
```

## 技术栈

- .NET 10
- Entity Framework Core 10
- SQL Server
- 前端：原生 HTML/CSS/JavaScript

## 数据库配置

在 `appsettings.json` 中配置 SQL Server 连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AIServeDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

## 运行项目

### 1. 还原依赖
```bash
cd e:\test
dotnet restore
```

### 2. 编译并运行
```bash
cd API/AIServe.API
dotnet run
```

## 访问地址

- 主页面: http://localhost:5000
- 登录页面: http://localhost:5000/login.html
- Swagger 文档: http://localhost:5000/swagger

## 默认账号

- 用户名: `admin`
- 密码: `admin123`

## API 接口说明

所有接口统一通过 `ApiService` 入口，通过 `action` 参数区分不同操作：

### 预约管理

| Action | Method | 描述 |
|--------|--------|------|
| `reservation_list` | GET | 获取预约列表 |
| `reservation_get` | GET | 获取预约详情（需传 `id` 参数） |
| `reservation_save` | POST | 创建/更新预约（推荐使用，不传 id 或 id <= 0 则创建，否则更新） |
| `reservation_updatestatus` | POST | 更新预约状态 |
| `reservation_delete` | GET | 删除预约（需传 `id` 参数） |

### 系统设置

| Action | Method | 描述 |
|--------|--------|------|
| `setup_getconfig` | GET | 获取系统配置 |
| `setup_updateconfig` | POST | 更新系统配置 |
| `setup_healthcheck` | GET | 健康检查 |

### 登录注册

| Action | Method | 描述 |
|--------|--------|------|
| `login_login` | POST | 用户登录 |
| `login_logout` | POST | 用户退出 |
| `login_register` | POST | 用户注册 |

接口详细文档请参考 [doc/接口文档.md](doc/接口文档.md)
