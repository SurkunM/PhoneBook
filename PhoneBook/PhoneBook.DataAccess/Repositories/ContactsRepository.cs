﻿using Microsoft.EntityFrameworkCore;
using PhoneBook.Contracts.Dto;
using PhoneBook.Contracts.Repositories;
using PhoneBook.DataAccess.Repositories.BaseAbstractions;
using PhoneBook.Model;

namespace PhoneBook.DataAccess.Repositories;

public class ContactsRepository : BaseEfRepository<Contact>, IContactsRepository
{
    public ContactsRepository(PhoneBookDbContext dbContext) : base(dbContext) { }

    public List<ContactDto> GetContacts()
    {
        return _dbSet
            .AsNoTracking()
            .Select(c => new ContactDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,                
                Phone = c.Phone,
            })
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)            
            .ToList();
    }

    public void CreateContact(ContactDto contactDto)
    {
        _dbSet.Add(new Contact
        {
            Id = contactDto.Id,
            FirstName = contactDto.FirstName,
            LastName = contactDto.LastName,
            Phone = contactDto.Phone
        });
    }
}
