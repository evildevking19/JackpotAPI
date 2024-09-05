using Microsoft.AspNetCore.Mvc;

using ApiServer.Models;
using ApiServer.Services;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChampionController(IChampionService championService) : ControllerBase
{
    [HttpGet]
    [Route("fetchAll")]
    public async Task<ActionResult> GetAll() {
        return Ok(await championService.GetAll());
    }

    [HttpGet]
    [Route("getChampion")]
    public async Task<ActionResult> GetChampion(int id) {
        return Ok(await championService.GetChampion(id, Request));
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] Champion champion) {
        return Ok(await championService.Create(champion));
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Champion champion) {
        return Ok(await championService.Update(champion));
    }

    [HttpDelete]
    [Route("delete")]
    public async Task<ActionResult> Delete(int id) {
        return Ok(await championService.Delete(id));
    }
}


