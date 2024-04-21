using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskmanAPI.Contexts;
using Microsoft.AspNetCore.Authorization;
using TaskmanAPI.Models;
using TaskmanAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication();
//builder.Services.AddSingleton<IAuthorizationPolicyProvider, RolePerProjectPolicyProvider>();
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DefaultContext>(options =>
{
    // use postgresql
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDefaultIdentity<TaskmanAPIUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TaskmanAPIContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();