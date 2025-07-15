using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResourceService.Application.Common.Models;
using ResourceService.Application.Resource.Commands;
using ResourceService.Application.ResourceReservation.Commands;
using ResourceService.Application.ResourceReservation.Queries;

namespace ResourceService.Api.Controllers;

public class ResourceReservationController : BaseController
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateResourceReservation([FromBody] CreateResourceReservationCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("{id}")]

public async Task<IActionResult> DeleteResourceReservation(long id)
    {
         await Mediator.Send(new DeleteResourceReservationCommand(id));
         return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllReservations(
        [FromQuery] List<long>? ids,
        [FromQuery] long resourceId,
        [FromQuery] Guid reservedBy,
        [FromQuery] Guid taskItemId,
        [FromQuery] DateTime? startTime,
        [FromQuery] DateTime? endTime,
        [FromQuery] PageQueryRequest pageQueryRequest)
    {
        var query = new GetAllResourceReservationsQuery(
            ids,
            resourceId,
            reservedBy,
            taskItemId,
            startTime ?? DateTime.MinValue,
            endTime ?? DateTime.MaxValue,
            pageQueryRequest);

        var result = await Mediator.Send(query);
        return Ok(result);
    }
}