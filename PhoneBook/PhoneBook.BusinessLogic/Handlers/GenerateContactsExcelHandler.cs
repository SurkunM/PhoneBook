using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhoneBook.BusinessLogic.Services;
using PhoneBook.Contracts.IRepositories;
using PhoneBook.Contracts.IUnitOfWork;

namespace PhoneBook.BusinessLogic.Handlers;

public class GenerateContactsExcelHandler
{
    private readonly ExcelGenerateService _excelService;

    private readonly IUnitOfWork _unitOfWork;

    public GenerateContactsExcelHandler(ExcelGenerateService excelService, IUnitOfWork unitOfWork)
    {
        _excelService = excelService ?? throw new ArgumentNullException(nameof(excelService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<MemoryStream> ExcelGenerateHandleAsync()
    {
        var contactsRepository = _unitOfWork.GetRepository<IContactsRepository>();

        var contacts = await contactsRepository.GetContactsAsync();

        return _excelService.GenerateContactsExcel(contacts);
    }

    public Task SaveContactsToExcelFileAsync(MemoryStream excelMemoryStream, IConfiguration fileName)
    {
        return _excelService.SaveContactsToExcelFileAsync(excelMemoryStream, fileName);
    }

    public FileStreamResult CreateExcelFileResult(MemoryStream stream, string fileName)
    {
        return _excelService.CreateExcelFileResult(stream, fileName);
    }
}