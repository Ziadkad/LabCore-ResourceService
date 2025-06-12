using Microsoft.EntityFrameworkCore;
using ResourceService.Api.Security;
using ResourceService.Application;
using ResourceService.Infrastructure;
using ResourceService.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.RegisterDataServices(builder.Configuration);
builder.Services.RegisterApplicationServices();
builder.Services.RegisterAuthServices(builder.Configuration);
builder.Services.RegisterSwaggerServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

MigrateDbToLatestVersion(app);


await app.RunAsync();


static void MigrateDbToLatestVersion(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
    using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.MigrateAsync();
}