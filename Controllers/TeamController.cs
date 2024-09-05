using Microsoft.AspNetCore.Mvc;

using ApiServer.Models;
using ApiServer.Services;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamController(ITeamService teamService) : ControllerBase
{
    [HttpGet]
    [Route("fetchAll")]
    public async Task<ActionResult> GetAll() {
        return Ok(await teamService.GetAll());
    }

    [HttpGet]
    [Route("getTeam")]
    public async Task<ActionResult> GetTeam(int id) {
        return Ok(await teamService.GetTeam(id, Request));
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] Team team) {
        return Ok(await teamService.Create(team));
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Team team) {
        return Ok(await teamService.Update(team));
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> Delete(int id) {
        return Ok(await teamService.Delete(id));
    }
}


