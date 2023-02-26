using dbCompanyTest.Environment;
using dbCompanyTest.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<HtmlEncoder>(
     HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin,
                                               UnicodeRanges.CjkUnifiedIdeographs })
    );
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAll",
        builder => builder.SetIsOriginAllowed(a => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(op =>
{
    op.IdleTimeout = TimeSpan.FromDays(1);
    op.Cookie.HttpOnly = true;
    op.Cookie.IsEssential = true;
}
    );
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();



app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<chatHub>("/chatHub");
chatHub.Current = ((IApplicationBuilder)app).ApplicationServices.GetService<IHubContext<chatHub>>();
string url = new dbCompanyTest.Environment.Environment().getEnvironment();
if (url != "https://localhost:7100")
    dbCompanyTest.Environment.Environment.useEnvironment = url;
app.Run();