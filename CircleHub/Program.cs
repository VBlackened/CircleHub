using CircleHub.Client.Components.Pages;
using CircleHub.Client.Services.Interfaces;
using CircleHub.Components;
using CircleHub.Components.Account;
using CircleHub.Configuration;
using CircleHub.Data;
using CircleHub.Services;
using CircleHub.Services.DemoUser;
using CircleHub.Services.Email;
using CircleHub.Services.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
//builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

builder.Services.AddScoped<DemoUserActivityFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<DemoUserActivityFilter>();
});

builder.Services.AddHttpClient();

builder.Services.AddOutputCache();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = DataUtility.GetConnectionString(builder.Configuration) ?? throw new InvalidOperationException("Connection string 'DbConnection' not found.");

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

//Resend service
builder.Services.AddOptions<ResendOptions>()
    .Bind(builder.Configuration.GetSection(ResendOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "Resend ApiKey is missing.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.SystemFrom), "Resend SystemFrom address is missing.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.ContactFrom), "Resend ContactFrom address is missing.")
    .ValidateOnStart();

var resendOptions = builder.Configuration
    .GetSection(ResendOptions.SectionName)
    .Get<ResendOptions>()
    ?? throw new InvalidOperationException("Resend configuration is missing.");

builder.Services.AddResend(options =>
{
    options.ApiToken = resendOptions.ApiKey;
});

builder.Services.AddTransient<IEmailSender<ApplicationUser>, ResendIdentityEmailSender>();

builder.Services.AddScoped<IEmailService, ResendEmailService>();

//Repositories
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

//DTO Services
builder.Services.AddScoped<ICategoryDTOService, CategoryDTOService>();
builder.Services.AddScoped<IContactDTOService, ContactDTOService>();

//Demo User Service
builder.Services.AddScoped<IDemoUserService, DemoUserService>();
builder.Services.AddScoped<IDemoUserActivityService, DemoUserActivityService>();

// Demo User Cleanup
builder.Services.AddHostedService<DemoUserCleanupService>();

var app = builder.Build();

var scope = app.Services.CreateScope();
await DataUtility.ManageDataAsync(scope.ServiceProvider);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CircleHub.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.MapControllers();

app.Run();
