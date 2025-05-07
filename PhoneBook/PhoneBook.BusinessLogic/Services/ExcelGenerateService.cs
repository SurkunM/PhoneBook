using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PhoneBook.Contracts.Dto;

namespace PhoneBook.BusinessLogic.Services;

public class ExcelGenerateService
{
    public MemoryStream GenerateContactsExcel(IEnumerable<ContactDto> contactsDto)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet();

        worksheet.Range("A1:C1").Merge();
        worksheet.Cell("A1").SetValue("Контакты");
        worksheet.Cell("A1").Style.Font.FontSize = 12;
        worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var contacts = contactsDto
             .Select(c => new
             {
                 c.FirstName,
                 c.LastName,
                 c.Phone
             })
             .ToList();

        var table = worksheet.Cell("A2").InsertTable(contacts, "Контакты");

        table.Field("LastName").Name = "Фамилия";
        table.Field("FirstName").Name = "Имя";
        table.Field("Phone").Name = "Телефон";

        table.Rows().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        table.ShowAutoFilter = false;
        table.Theme = XLTableTheme.TableStyleMedium5;

        worksheet.Columns().AdjustToContents();

        var excelMemoryStream = new MemoryStream();
        workbook.SaveAs(excelMemoryStream);

        excelMemoryStream.Position = 0;

        return excelMemoryStream;
    }

    public async Task SaveContactsExcel(MemoryStream excelMemoryStream, IConfiguration configuration)
    {
        var path = Path.Combine(configuration["ExcelExport:Path"] ?? throw new InvalidOperationException("Указан некорректный путь для сохранения файла"),
            $"contacts_{DateTime.Now:yyyyMMddHHmmss}.xlsx");

        var directory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException($"Некорректный путь: {path}");
        Directory.CreateDirectory(directory);

        await File.WriteAllBytesAsync(path, excelMemoryStream.ToArray());
    }

    public FileStreamResult CreateExcelFileResult(MemoryStream stream, string fileName)
    {
        if (stream is null || stream.Length == 0)
        {
            throw new ArgumentException($"Переданный поток пусть. {stream}");
        }

        stream.Position = 0;

        return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
        {
            FileDownloadName = fileName
        };
    }
}