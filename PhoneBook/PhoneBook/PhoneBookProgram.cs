using Microsoft.EntityFrameworkCore;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.DataAccess;

namespace PhoneBook;

public class PhoneBookProgram
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<PhoneBookDbContext>(options =>
        {
            options
                .UseSqlServer(builder.Configuration.GetConnectionString("PhoneBookConnection"))
                .UseLazyLoadingProxies();
        }, ServiceLifetime.Transient, ServiceLifetime.Transient);

        builder.Services.AddControllersWithViews();
        builder.Services.AddTransient<DbInitializer>();
        builder.Services.AddTransient<GetContactsHandler>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            try
            {
                var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
                dbInitializer.Initialize();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<PhoneBookProgram>>();
                logger.LogError(ex, "При отправке базы данных произошла ошибка.");

                throw;
            }
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts. 43:38
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }
}