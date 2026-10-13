using SmartRecruitment.Application.ApplicationExstintion;
using SmartRecruitment.Domain.Interfaces;
using SmartRecruitment.Infrastructure.Extensions;
using SmartRecruitment.Infrastructure.Seeder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// 1. ≈÷«›… ”Ì«”… «·‹ CORS ··”„«Õ ··›—Ê‰  ≈‰œ »«·« ’«·
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("http://localhost:60725", "http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var unitOfWork = services.GetRequiredService<IUnitOfWork>();
    await DbSeeder.SeedAdminUserAsync(unitOfWork);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2.  ›⁄Ì· «·‹ CORS Middleware (ÌÃ» √‰ ÌﬂÊ‰ ﬁ»· UseAuthorization Ê MapControllers)
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();