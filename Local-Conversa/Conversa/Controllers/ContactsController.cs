using Conversa.Models.Databases.Dtos;
using Conversa.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Conversa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] AddContactRequest request)
        {
            if (request.UserTelephone == string.Empty || request.ContactTelephone == string.Empty || string.IsNullOrEmpty(request.Nickname))
            {
                return BadRequest(new { message = "No ha sido posible agregar como contacto a este usuario" });
            }

            bool isAdded = await _contactRepository.AddContactByPhoneAsync(request.UserTelephone, request.ContactTelephone, request.Nickname);

            if (isAdded)
            {
                return Ok(new { message = "Contacto agregado la agenda." });
            }
            else
            {
                return Conflict(new { message = "Ya tienes a esta persona agregada a tu lista de contactos" });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetContacts(Guid userId)
        {
            var contacts = await _contactRepository.GetContactsByUserIdAsync(userId);

            if (contacts == null || contacts.Count == 0)
            {
                return NotFound(new { message = "Aún no tienes contactos agregados." });
            }

            return Ok(contacts);
        }
    }
}
