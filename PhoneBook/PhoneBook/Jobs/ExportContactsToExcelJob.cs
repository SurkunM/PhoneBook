﻿using PhoneBook.BusinessLogic.Handlers;

namespace PhoneBook.Jobs;

public class ExportContactsToExcelJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IConfiguration _configuration;

    private readonly ILogger<ExportContactsToExcelJob> _logger;

    public ExportContactsToExcelJob(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<ExportContactsToExcelJob> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)) ;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
#if DEBUG
            await ExportProcessAsync();
#endif
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

        var excelMemoryStream = await handler.ExcelGenerateHandleAsync();

        await handler.SaveContactsToExcelFileAsync(excelMemoryStream, _configuration);
    }
}
