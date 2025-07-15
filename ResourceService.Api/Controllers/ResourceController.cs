using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceService.Application.Common.Exceptions;
using ResourceService.Application.Common.Models;
using ResourceService.Application.Resource.Commands;
using ResourceService.Application.Resource.Queries;
using ResourceService.Domain.Resource;

namespace ResourceService.Api.Controllers;

public class ResourceController : BaseController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateResource([FromBody] CreateResourceCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateResource(long id, [FromBody] UpdateResourceCommand command)
    {
        if (id != command.Id)
        {
            throw new BadRequestException("Ids do not match");
        }
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteResource(long id)
    {
        await Mediator.Send(new DeleteResourceCommand(id));
        return NoContent();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetResource(
        long id,
        [FromQuery] List<Guid>? reservedByList,
        [FromQuery] Guid? taskItemId,
        [FromQuery] DateTime? startTime,
        [FromQuery] DateTime? endTime)
    {
        var query = new GetResourceQuery(
            Id: id,
            ReservedByList: reservedByList,
            TaskItemId: taskItemId,
            StartTime: startTime,
            EndTime: endTime
        );

        var result = await Mediator.Send(query);
        return Ok(result);
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetResources(
        [FromQuery] List<long>? ids,
        [FromQuery] string? keyword,
        [FromQuery] ResourceType? type,
        [FromQuery] ResourceStatus? status,
        [FromQuery] PageQueryRequest pageQuery)
    {
        var query = new GetAllResourcesQuery(ids, keyword, type, status, pageQuery);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

}