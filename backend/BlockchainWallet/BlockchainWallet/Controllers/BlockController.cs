using BlockchainWallet.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainWallet.Controllers
{
    public class BlockController : Controller
    {
        private BlockRepository _repository;
        private readonly ILogger<BlockController> _logger;

        public BlockController(BlockRepository repository, ILogger<BlockController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("api/block/{hash}")]
        public async Task<IActionResult> GetByHash(string hash)
        {
            _logger.LogInformation("GETTING block with hash: {BlockHash}", hash);

            try
            {
                if (string.IsNullOrEmpty(hash))
                {
                    return BadRequest("Hash cannot be empty");
                }
                return Ok(await _repository.GetBlockByHashAsync(hash));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error retrieving block with hash: {BlockHash}", hash);
                return StatusCode(500, $"Internal error, Exeption: {e}");
            }
        }
    }
}