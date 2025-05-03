using System.ComponentModel.DataAnnotations;

namespace PhoneBook.Contracts.Dto;

public class GetContactsQueryParameters
{
    private string _term = string.Empty;

    private string _sortBy = "FirstName";

    [MaxLength(100)]
    public string Term
    {
        get => _term;
        set => _term = value ?? string.Empty;
    }

    [MaxLength(50)]
    public string SortBy {
        get => _sortBy;
        set => _sortBy = value ?? _sortBy;
    }

    public bool IsDescending { get; set; } = false;

    [Range(1, int.MaxValue, ErrorMessage = "Передано не корректное значение номера страницы")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "Передано не корректное значение размера страницы")]
    public int PageSize { get; set; } = 10;
}
