using Dfe.PlanTech.Application.Helpers;
using Dfe.PlanTech.Web;
using Dfe.PlanTech.Infrastructure.SignIn;
using Dfe.PlanTech.Web.Helpers;
using Dfe.PlanTech.Web.Middleware;
using GovUk.Frontend.AspNetCore;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);
IdentityModelEventSource.ShowPII = true;
builder.Services.AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddGovUkFrontend();

builder.Services.AddCaching();
builder.Services.AddCQRSServices();
builder.Services.AddContentfulServices(builder.Configuration);
builder.Services.AddDfeSignIn(builder.Configuration);
builder.Services.AddScoped<ComponentViewsFactory>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UrlHistoryMiddleware>();

app.MapControllerRoute(
    name: "questionsController",
    pattern: "question/{action=GetQuestionById}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Pages}/{action=GetByRoute}/{id?}");

app.Run();