using Conversa.Models.Databases.Dtos;
using Conversa.Models.Databases.Entities;
using Conversa.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Conversa.Models.Databases.Repository
{
    public class ContactsRepository : IContactRepository
    {
        private readonly DataContext _context;

        public ContactsRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddContactByPhoneAsync(string userPhone, string contactPhone, string nickname)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == userPhone);
            var contact = await _context.Users.FirstOrDefaultAsync(u => u.Phone == contactPhone);

            if (user == null || contact == null)
            {
                return false; 
            }

            bool alreadyExists = await _context.Contacts.AnyAsync(c => c.UserId == user.Id && c.ContactId == contact.Id);
            if (alreadyExists)
            {
                return false; 
            }

            var newContact = new Contacts
            {
                UserId = user.Id,
                ContactId = contact.Id,
                Nickname = nickname,
                DateAdded = DateTime.UtcNow
            };

            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ContactDto>> GetContactsByUserIdAsync(Guid userId)
        {
            var contacts = await _context.Contacts
                .Where(c => c.UserId == userId)
                .Include(c => c.ContactUser)
                .Select(c => new ContactDto
                {
                    ContactId = c.ContactUser.Id,
                    Nickname = c.Nickname,
                    DateAdded = c.DateAdded
                })
                .ToListAsync();

            return contacts;
        }
    }
}

