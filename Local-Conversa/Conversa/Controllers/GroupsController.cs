using Conversa.Models.Databases.Dtos;
using Conversa.Models.Interfaces;
using Conversa.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Conversa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
    {
        try
        {
            var groupDto = await _groupService.CreateGroupWithContactsAsync(
                dto.CreatorId,
                dto.Name,
                dto.Description,
                dto.ContactIds
            );

            return Ok(groupDto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}


