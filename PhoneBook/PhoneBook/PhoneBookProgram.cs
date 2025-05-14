using Microsoft.EntityFrameworkCore;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.BusinessLogic.Services;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;
using PhoneBook.DataAccess;
using PhoneBook.DataAccess.Repositories;
using PhoneBook.DataAccess.UnitOfWork;
using PhoneBook.Jobs;

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
        }, ServiceLifetime.Scoped, ServiceLifetime.Transient);

        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<DbInitializer>();

        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
        builder.Services.AddTransient<IContactsRepository, ContactsRepository>();

        builder.Services.AddTransient<GetContactsHandler>();
        builder.Services.AddTransient<CreateContactHandler>();
        builder.Services.AddTransient<UpdateContactHandler>();
        builder.Services.AddTransient<DeleteContactHandler>();
        builder.Services.AddTransient<GenerateContactsExcelHandler>();

        builder.Services.AddTransient<ExcelGenerateService>();

        builder.Services.AddHostedService<ExportContactsToExcelJob>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            try
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
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