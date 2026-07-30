using AlisPo.Api.Data;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace AlisPo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestDbController : ControllerBase
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TestDbController(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    [HttpGet]
    public IActionResult Get()
    {
        using var connection = _connectionFactory.CreateConnection();

        var currentTime = connection.QueryFirst<string>(
            "SELECT CONVERT(varchar, GETDATE(), 120)");

        return Ok(new
        {
            Message = "Database Connected Successfully",
            ServerTime = currentTime
        });
    }
}