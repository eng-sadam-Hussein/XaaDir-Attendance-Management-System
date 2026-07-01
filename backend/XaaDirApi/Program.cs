using XaaDirApi.Data;
using XaaDirApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddScoped<AuthRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClassRepository>();
builder.Services.AddScoped<SubjectRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<AttendanceRepository>();
builder.Services.AddScoped<ReportRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Local development: keep HTTP enabled to avoid browser certificate errors.
// app.UseHttpsRedirection();

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "XaaDir Attendance Management System API is running.");
app.Run();
