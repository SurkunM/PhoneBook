using Microsoft.EntityFrameworkCore;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess;
using PhoneBook.DataAccess.Repositories;

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
        builder.Services.AddTransient<IContactsRepository, ContactsRepository>();

        builder.Services.AddTransient<GetContactsHandler>();
        builder.Services.AddTransient<CreateContactHandler>();
        builder.Services.AddTransient<UpdateContactHandler>();
        builder.Services.AddTransient<DeleteContactHandler>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
                dbInitializer.Initialize();
            }
            catch (Exception ex)
            {
                var logger = app.Services.GetRequiredService<ILogger<PhoneBookProgram>>();
                logger.LogError(ex, "При создании базы данных произошла ошибка.");

                throw;
            }
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
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