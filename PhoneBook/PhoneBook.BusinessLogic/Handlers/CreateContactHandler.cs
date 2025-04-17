﻿using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;

namespace PhoneBook.BusinessLogic.Handlers;

public class CreateContactHandler
{
    private readonly IContactsRepository _contactsRepository;

    public CreateContactHandler(IContactsRepository contactsRepository)
    {
        _contactsRepository = contactsRepository ?? throw new ArgumentNullException(nameof(contactsRepository));
    }

    public void Handle(ContactDto contactDto)
    {
         _contactsRepository.CreateContact(contactDto);
    }
}
