using DattingApp.API.Models;
using DattingApp.API.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace DattingApp.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IValuesRepository _valuesRepository;

    public ValuesController(IValuesRepository valuesRepository)
    {
        _valuesRepository = valuesRepository ??
                throw new ArgumentNullException(nameof(valuesRepository));
    }

    [HttpGet]
    public async Task<IEnumerable<Value>> Get()
    {
        return await _valuesRepository.GetAllValuesAsync();
    }

    [HttpGet("{id:int}", Name = "GetValueById")]
    public async Task<ActionResult<Value>> Get(int id)
    {
        var resultFromDatabase = await _valuesRepository.GetValueByIdAsync(id);
        return resultFromDatabase is null ?
            NotFound() : resultFromDatabase;
    }

    [HttpPost]
    public async Task<ActionResult> Post(Value value)
    {
        var result = await _valuesRepository.AddAsync(value);
        return result ? CreatedAtRoute("GetValueById",new { value.Id }, value) : BadRequest();
    }
}