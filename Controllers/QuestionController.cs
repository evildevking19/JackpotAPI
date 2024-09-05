using Microsoft.AspNetCore.Mvc;

using ApiServer.Models;
using ApiServer.Services;
using System.Text.Json;

namespace ApiServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController(IQuestionService questionService) : ControllerBase
{
    [HttpGet]
    [Route("fetchAll")]
    public async Task<ActionResult> GetAll() {
        return Ok(await questionService.GetAll());
    }

    [HttpGet]
    [Route("getQuestion")]
    public async Task<ActionResult> GetQuestion(int id) {
        return Ok(await questionService.GetQuestion(id, Request));
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] Question question) {
        return Ok(await questionService.Create(question));
    }

    [HttpPut]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] Question question) {
        return Ok(await questionService.Update(question));
    }
}


