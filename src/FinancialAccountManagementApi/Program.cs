using System.Net;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using FinancialAccountManagementApi.Configurations;
using Microsoft.AspNetCore.Localization;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddFluentValidation();
MapsterConfig.Configure();
builder.Services.AddExceptionMiddleware();
builder.Services.AddDbContext(builder.Configuration);
builder.Services.AddDataSeeder(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddSwagger();
builder.Services.AddControllers(mvcOptions => mvcOptions
    .AddResultConvention(resultStatusMap => resultStatusMap
        .AddDefaultMap()
        .For(ResultStatus.Ok, HttpStatusCode.OK, resultStatusOptions => resultStatusOptions
            .For("POST", HttpStatusCode.Created)
            .For("DELETE", HttpStatusCode.NoContent))
        .For(ResultStatus.Error, HttpStatusCode.BadRequest)
    ));


builder.Services.AddDateOnlyTimeOnlyStringConverters();
builder.Services.AddEndpointsApiExplorer();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.UseDataSeeder();
}
else
{
    app.UseHsts();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en")
});
app.UseExceptionMiddleware();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }