using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<GatewayController> _logger;
        private readonly IAuthenticationService _authenticationService;

        public GatewayController(DaprClient daprClient, ILogger<GatewayController> logger, IAuthenticationService authenticationService)
        {
            _daprClient = daprClient;
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpPost("command/{commandName}")]
        public async Task<IActionResult> HandleCommand(string commandName, [FromBody] object commandData)
        {
            // Authenticate the request (example)
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (!_authenticationService.Authenticate(token))
            {
                return Unauthorized();
            }

            // Generate TraceId
            var traceId = Guid.NewGuid().ToString();
            HttpContext.Request.Headers["TraceId"] = traceId;

            // Log the request
            _logger.LogInformation("Received command {CommandName} with TraceId: {TraceId}", commandName, traceId);

            // Forward the command to the CommandService via Dapr
            var result = await _daprClient.InvokeMethodAsync<object, object>(
                HttpMethod.Post,
                "commandservice",  // Dapr service ID
                commandName,
                commandData);

            return Ok(result);
        }

        [HttpGet("query/{queryName}")]
        public async Task<IActionResult> HandleQuery(string queryName)
        {
            // Authenticate the request (example)
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            if (!_authenticationService.Authenticate(token))
            {
                return Unauthorized();
            }

            // Generate TraceId
            var traceId = Guid.NewGuid().ToString();
            HttpContext.Request.Headers["TraceId"] = traceId;

            // Log the request
            _logger.LogInformation("Received query {QueryName} with TraceId: {TraceId}", queryName, traceId);

            // Forward the query to the QueryService via Dapr
            var result = await _daprClient.InvokeMethodAsync<object>(
                HttpMethod.Get,
                "queryservice",  // Dapr service ID
                queryName);

            return Ok(result);
        }
    }
}
