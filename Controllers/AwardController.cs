using Microsoft.AspNetCore.Mvc;

using ApiServer.Models;
using ApiServer.Services;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AwardController(IAwardService awardService) : ControllerBase
{
    [HttpGet]
    [Route("fetchAll")]
    public async Task<ActionResult> GetAll() {
        return Ok(await awardService.GetAll());
    }

    [HttpGet]
    [Route("getAward")]
    public async Task<ActionResult> GetAward(int id) {
        return Ok(await awardService.GetAward(id, Request));
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] Award award) {
        return Ok(await awardService.Create(award));
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Award award) {
        return Ok(await awardService.Update(award));
    }
}


