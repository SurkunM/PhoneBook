using PhoneBook.BusinessLogic.Handlers;

namespace PhoneBook.Jobs;

public class ExportContactsToExcelJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    private readonly ILogger<ExportContactsToExcelJob> _logger;

    public ExportContactsToExcelJob(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ExportContactsToExcelJob> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ExportProcessAsync();

            _logger.LogInformation("Произведена выгрузка контактов при старте приложения.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                await ExportProcessAsync();

                _logger.LogInformation("Произведена плановая (один раз в сутки) выгрузка контактов.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при выгрузке контактов в фоновой задаче.");
        }
    }

    private async Task ExportProcessAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<GenerateContactsExcelHandler>();

        var excelMemoryStream = await handler.ExcelGenerateHandlerAsync();

        await handler.SaveContactsExcel(excelMemoryStream, _configuration);
    }
}
