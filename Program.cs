var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SmartProcessAI.Services.MotorReglasService>();
builder.Services.AddSingleton<SmartProcessAI.Services.AnalisisMLService>();

var app = builder.Build();
var ml = app.Services.GetRequiredService<SmartProcessAI.Services.AnalisisMLService>();
ml.EntrenarModelos(Path.Combine(app.Environment.ContentRootPath, "Datos", "tareas_ejemplo.csv"));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
