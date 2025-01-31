using Business;
using DSTemplate_UI.Services;
using Fucktor.Controllers;
using Fucktor.Utils;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddBusiness(connectionString);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddDSTemplateUI();


builder.Services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddMvc().AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(SharedResource));
});

//adding policies for bank gateways
builder.Services.AddCors(options =>
{
    options.AddPolicy("SamanPolicy",
        policy =>
        {
            policy.WithOrigins("https://sep.shaparak.ir")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    //app.UseDeveloperExceptionPage();
    app.UseExceptionHandler($"/{nameof(DashboardController).RemoveController()}/{nameof(DashboardController.Error)}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//set the default culture to fa-IR
var supportedCultures = new[]
{
    "fa-IR",
};

app.UseRequestLocalization(options =>
    options
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
        .SetDefaultCulture(supportedCultures[0])
);

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseBusiness();


app.MapControllerRoute(
    name: "default",
    pattern: $"{{controller={nameof(HomeController).RemoveController()}}}/{{action={nameof(HomeController.Index)}}}/{{id?}}");
app.MapRazorPages();

app.Run();
