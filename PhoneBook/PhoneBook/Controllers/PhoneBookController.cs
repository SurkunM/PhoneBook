﻿using Microsoft.AspNetCore.Mvc;
using PhoneBook.BusinessLogic.Handlers;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Responses;

namespace PhoneBook.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class PhoneBookController : ControllerBase
{
    private readonly CreateContactHandler _createContactHandler;

    private readonly GetContactsHandler _getContactsHandler;

    private readonly UpdateContactHandler _updateContactHandler;

    private readonly DeleteContactHandler _deleteContactHandler;

    private readonly GenerateContactsExcelHandler _generateContactsExcelHandler;

    private readonly ILogger<PhoneBookController> _logger;

    public PhoneBookController(
        GetContactsHandler getContactsHandler, CreateContactHandler createContactHandler,
        UpdateContactHandler updateContactHandler, DeleteContactHandler deleteContactHandler,
        GenerateContactsExcelHandler generateContactsExcelHandler, ILogger<PhoneBookController> logger)
    {
        _createContactHandler = createContactHandler ?? throw new ArgumentNullException(nameof(createContactHandler));
        _getContactsHandler = getContactsHandler ?? throw new ArgumentNullException(nameof(getContactsHandler));
        _updateContactHandler = updateContactHandler ?? throw new ArgumentNullException(nameof(updateContactHandler));
        _deleteContactHandler = deleteContactHandler ?? throw new ArgumentNullException(nameof(deleteContactHandler));
        _generateContactsExcelHandler = generateContactsExcelHandler ?? throw new ArgumentNullException(nameof(generateContactsExcelHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<ActionResult<PhoneBookPage>> GetContacts([FromQuery] GetContactsQueryParameters queryParameters)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! При запросе на получение контакта переданы не корректные параметры страницы. ");

            return BadRequest(ModelState);
        }

        try
        {
            var contacts = await _getContactsHandler.HandleAsync(queryParameters);

            return Ok(contacts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Запрос на получение контактов не выполнен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact(ContactDto contactDto)
    {
        if (contactDto is null)
        {
            _logger.LogError("Ошибка! Объект ContactDto пуст.");

            return BadRequest("Объект \"Новый контакт\" пуст.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Переданы не корректные данные для создания контакта. {ContactDto}", contactDto);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isCreated = await _createContactHandler.HandleAsync(contactDto);

            if (!isCreated)
            {
                return BadRequest("Ошибка! Контакт с таким номером уже существует.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Контакт не создан.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateContact(ContactDto contactDto)
    {
        if (contactDto is null)
        {
            _logger.LogError("Ошибка! Объект ContactDto пуст.");

            return BadRequest("Объект ContactDto пуст.");
        }

        if (!ModelState.IsValid)
        {
            _logger.LogError("Ошибка! Не корректно заполнены поля для изменения контакта. {ContactDto}", contactDto);

            return UnprocessableEntity(ModelState);
        }

        try
        {
            var isUpdated = await _updateContactHandler.HandleAsync(contactDto);

            if (!isUpdated)
            {
                return BadRequest("Ошибка! Контакт с таким номером уже существует.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Контакт не изменен.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteContact([FromBody] int id)
    {
        if (id < 0)
        {
            _logger.LogError("Передано значение id меньше нуля. id={id}", id);

            return BadRequest("Передано некорректное значение.");
        }

        try
        {
            var isDeleted = await _deleteContactHandler.DeleteSingleContactHandleAsync(id);

            if (!isDeleted)
            {
                _logger.LogError("Ошибка! Контакт для удаления не найден. id={id}", id);

                return BadRequest("Контакт для удаления не найден.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление контакта не выполнено. id={id}", id);

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllSelectedContact([FromBody] List<int> selectedContactsId)
    {
        if (selectedContactsId is null || selectedContactsId.Count == 0)
        {
            _logger.LogError("Ошибка! Не переданы данные для удаления.{ContactsId}", selectedContactsId);

            return BadRequest("Не переданы данные для удаления.");
        }

        try
        {
            await _deleteContactHandler.DeleteAllSelectedContactHandleAsync(selectedContactsId);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка! Удаление выбранных контактов не выполнено.");

            return StatusCode(StatusCodes.Status500InternalServerError, "Ошибка сервера.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportToExcel()
    {
        try
        {
            var memoryStream = await _generateContactsExcelHandler.ExcelGenerateHandleAsync();

            return _generateContactsExcelHandler.CreateExcelFileResult(memoryStream, $"contacts_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при экспорте в Excel");

            return StatusCode(StatusCodes.Status500InternalServerError, "Произошла ошибка при генерации файла");
        }
    }
}