using Microsoft.AspNetCore.Mvc;

using ApiServer.Models;
using ApiServer.Services;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JackpotController(IJackpotService jackpotService) : ControllerBase
{
    [HttpGet]
    [Route("fetchAll")]
    public async Task<ActionResult> GetAll() {
        return Ok(await jackpotService.GetAll());
    }

    [HttpGet]
    [Route("getJackpot")]
    public async Task<ActionResult> GetJackpot(int id) {
        return Ok(await jackpotService.GetJackpot(id, Request));
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] Jackpot jackpot) {
        return Ok(await jackpotService.Create(jackpot));
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Jackpot jackpot) {
        return Ok(await jackpotService.Update(jackpot));
    }
}


