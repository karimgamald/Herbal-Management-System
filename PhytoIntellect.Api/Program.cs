using PhytoIntellect.Api.Extensions;
using PhytoIntellect.Application;
using PhytoIntellect.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllerServices();
builder.Services.AddSwaggerServices();
builder.Services.AddCorsServices();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddDbContextServices(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);


builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
