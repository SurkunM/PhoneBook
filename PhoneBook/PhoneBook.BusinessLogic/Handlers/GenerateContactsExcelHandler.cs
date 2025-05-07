using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhoneBook.BusinessLogic.Services;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.BusinessLogic.Handlers;

public class GenerateContactsExcelHandler
{
    private readonly ExcelGenerateService _excelService;

    private readonly IContactsRepository _contactsRepository;

    public GenerateContactsExcelHandler(ExcelGenerateService excelService, IContactsRepository contactsRepository)
    {
        _excelService = excelService;
        _contactsRepository = contactsRepository;
    }

    public async Task<MemoryStream> ExcelGenerateHandlerAsync()
    {
        var contacts = await _contactsRepository.GetContactsAsync();

        return _excelService.GenerateContactsExcel(contacts);
    }

    public Task SaveContactsExcel(MemoryStream excelMemoryStream, IConfiguration fileName)
    {
        return _excelService.SaveContactsExcel(excelMemoryStream, fileName);
    }

    public FileStreamResult CreateExcelFileResult(MemoryStream stream, string fileName)
    {
        return _excelService.CreateExcelFileResult(stream, fileName);
    }
}