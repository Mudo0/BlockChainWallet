using BlockchainWallet.Config;
using BlockchainWallet.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BlockchainWallet.Controllers
{
    public class TestController : Controller
    {
        private readonly IRepository repository;
        private readonly ILogger<TestController> _logger;

        public TestController(IRepository repository, ILogger<TestController> ilogger)
        {
            this.repository = repository;
            _logger = ilogger;
        }

        [HttpPost("/test/{name}&{age}")]
        public async Task<IActionResult> Create(string name, int age)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    return BadRequest("No puede estar vacio el nombre");
                }
                return Ok(await repository.AddPerson(name, age));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "EndpointError");
                throw;
            }
        }

        [HttpGet("/test/{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            try
            {
                return Ok(await repository.SearchPersonsByName(name));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "EndpointError");
                throw;
            }
        }
    }
}