using Microsoft.AspNetCore.Mvc;

using ApiServer.Models;
using ApiServer.Services;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PotController(IPotService potService) : ControllerBase
{
    [HttpGet]
    [Route("fetchAll")]
    public async Task<ActionResult> GetAll() {
        return Ok(await potService.GetAll());
    }

    [HttpGet]
    [Route("getPot")]
    public async Task<ActionResult> GetPot(int id) {
        return Ok(await potService.GetPot(id, Request));
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] Pot pot) {
        return Ok(await potService.Create(pot));
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Pot pot) {
        return Ok(await potService.Update(pot));
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> Delete(int id) {
        return Ok(await potService.Delete(id));
    }
}


