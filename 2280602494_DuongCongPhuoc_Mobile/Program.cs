using _2280602494_DuongCongPhuoc_Mobile.Models;
using _2280602494_DuongCongPhuoc_Mobile.Repositories;
using _2280602494_DuongCongPhuoc_Mobile.Services;
using _2280602494_DuongCongPhuoc_Mobile.Hubs;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Cấu hình: Dùng SQL Server làm hệ quản trị csdl cho ứng dụng

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Register identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventCategoryRepository, EventCategoryRepository>();
builder.Services.AddScoped<IReminderRepository, ReminderRepository>();
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IWeddingTaskRepository, WeddingTaskRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IEventVendorRepository, EventVendorRepository>();
builder.Services.AddScoped<IEventTimelineRepository, EventTimelineRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();

// Register SMS/OTP Services
builder.Services.AddSingleton<OTPService>();
builder.Services.AddSingleton<SpeedSMSService>();
builder.Services.AddScoped<VnPayLibrary>();
builder.Services.AddSignalR();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS: Cross-Origin Resource Sharing, được dịch là "Chia sẻ tài nguyên giữa các nguồn gốc khác nhau
builder.Services.AddCors(options =>

{

    options.AddPolicy(name: "MyAllowOrigins", policy =>

    {

        //Thay bằng địa chỉ localhost khi khởi chạy bên frontend (VSCode)
        //Cho phép tất cả origins trong development để Flutter app có thể kết nối
        if (builder.Environment.IsDevelopment())
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
        .AllowAnyHeader()
        .AllowAnyMethod();
        }
    });

});
//Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JWTKey");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();
//Tạo ra các Role trong ứng dụng để sau này thực hiện phân quyền
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Staff", "User" }; // 3 loại tài khoản: Admin, Staff (Nhân viên), User (Khách hàng)
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

if (!app.Environment.IsDevelopment())

{

    app.UseExceptionHandler("/error"); // Custom error handling endpoint

    app.UseHsts(); // Enforce HTTPS in production

}

else

{

    app.UseSwagger();

    app.UseSwaggerUI();
}

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Chỉ bật HTTPS redirection trong production để tránh warning trong development
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Áp dụng CORS cho các yêu cầu đến API
app.UseCors("MyAllowOrigins");

// UseAuthentication phải được gọi trước UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// Serve static frontend for testing
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<_2280602494_DuongCongPhuoc_Mobile.Hubs.ChatHub>("/chatHub");

//Người dùng đã tự config Identity user nên bỏ phần này
//app.MapIdentityApi<User>();

app.Run();
