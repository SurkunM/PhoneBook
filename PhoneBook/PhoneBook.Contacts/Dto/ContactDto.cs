﻿namespace PhoneBook.Contracts.Dto;

public class ContactDto
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string MiddleName { get; set; }

    public required List<PhoneNumberDto> PhoneNumbers { get; set; }
}
