using Conversa.Models.Databases;
using Conversa.Models.Databases.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Conversa.Models.Interfaces;

namespace Conversa.Seeders;

public class DatabaseSeeder
{
    private readonly DataContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public DatabaseSeeder(DataContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Aplica las migraciones si no están aplicadas
            await _context.Database.MigrateAsync();

            // Verifica si ya existen usuarios para evitar duplicados
            if (await _context.Users.AnyAsync())
            {
                Console.WriteLine("Ya existen usuarios en la base de datos. No se ejecuta el seeding.");
                return;
            }

            // 1. Crear usuarios con contraseñas hasheadas
            Console.WriteLine("Agregando usuarios...");
            var user1 = new Users
            {
                Id = Guid.NewGuid(),
                Phone = "111111111",
                Name = "Christian",
                Password = _passwordHasher.Hash("Christian123.")
            };

            var user2 = new Users
            {
                Id = Guid.NewGuid(),
                Phone = "222222222",
                Name = "Yasir",
                Password = _passwordHasher.Hash("Yasir123.")
            };

            _context.Users.AddRange(user1, user2);
            await _context.SaveChangesAsync();
            Console.WriteLine("Usuarios agregados exitosamente.");

            // 2. Crear contactos (para que user1 y user2 sean contactos mutuamente)
            Console.WriteLine("Agregando contactos...");
            var contact1 = new Contacts
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                ContactId = user2.Id,
                Nickname = "Yas",
                DateAdded = DateTime.UtcNow
            };

            var contact2 = new Contacts
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                ContactId = user1.Id,
                Nickname = "Chris",
                DateAdded = DateTime.UtcNow
            };

            _context.Contacts.AddRange(contact1, contact2);
            await _context.SaveChangesAsync();
            Console.WriteLine("Contactos agregados exitosamente.");

            // 3. Crear mensajes directos (sin grupo)
            Console.WriteLine("Agregando mensajes...");
            var messages = new List<Messages>
            {
                new Messages
                {
                    Id = Guid.NewGuid(),
                    SenderId = user1.Id,
                    ReceiverId = user2.Id,
                    GroupId = null, 
                    Content = "¡Hola Yasir!",
                    MessageType = "text",
                    SentAt = DateTime.UtcNow,
                    Status = "delivered"
                },
                new Messages
                {
                    Id = Guid.NewGuid(),
                    SenderId = user2.Id,
                    ReceiverId = user1.Id,
                    GroupId = null, 
                    Content = "¡Hola Christian! ¿Cómo estás?",
                    MessageType = "text",
                    SentAt = DateTime.UtcNow.AddMinutes(1),
                    Status = "read"
                }
            };

            _context.Messages.AddRange(messages);
            await _context.SaveChangesAsync();
            Console.WriteLine("Mensajes agregados exitosamente.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error durante el seeding: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw; // O maneja el error según tu necesidad
        }
    }
}

