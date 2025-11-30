using Microsoft.AspNetCore.Mvc;
using N5.Permissions.Application.Commands;
using N5.Permissions.Application.DTOs;
using N5.Permissions.Application.Queries;

namespace N5.Permissions.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly RequestPermissionHandler _requestHandler;
    private readonly ModifyPermissionHandler _modifyHandler;
    private readonly GetPermissionsHandler _getHandler;

    public PermissionsController(
        RequestPermissionHandler requestHandler,
        ModifyPermissionHandler modifyHandler,
        GetPermissionsHandler getHandler)
    {
        _requestHandler = requestHandler;
        _modifyHandler = modifyHandler;
        _getHandler = getHandler;
    }

    [HttpPost]
    public async Task<IActionResult> RequestPermission([FromBody] CreatePermissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _requestHandler.Handle(dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> ModifyPermission(int id, [FromBody] UpdatePermissionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _modifyHandler.Handle((id, dto));
        
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var result = await _getHandler.Handle();
        return Ok(result);
    }
}

