using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebApp.Models.Requests;
using WebApp.Models.Responses;

namespace WebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebController : ControllerBase
{
    private readonly ILogger<WebController> _logger;
    private IModel _model;


    public WebController(IModel model, ILogger<WebController> logger)
    {
        _logger = logger;
        _model = model;
    }
    /// <summary>
    /// Get list of available games
    /// </summary>
    /// <returns>Lits available games if success otherwise error message</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/games
    /// </remarks>
    /// <response code="200">List of available games with IDs.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("games")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<GameList> GetGames()
    {
        _logger.LogInformation("Request to get game list.");

        try
        {
            // TODO
            return new GameList([]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting game list: {id}",
                ex.Message);

            return StatusCode(500, new ErrorMessage($"Internal server error: {ex.Message}"));
        }
    }


    /// <summary>
    /// Start game with Id
    /// </summary>
    /// <param name="gameId"></param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/games/1
    /// </remarks>
    /// <response code="200">List of available games.</response>
    /// <response code="404">No game found with provided ID.</response>
    /// <response code="409">User had already started another game.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("games/{gameId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult StartGame([Required] int gameId)
    {
        _logger.LogInformation("Request to start game with ID: {id}",
            gameId);

        try
        {
            // TODO ADD CHOOSING GAME
            _model = null;
            return Ok();
            //return NotFound(new ErrorMessage($"No game found with ID {gameId}"));
            //return Conflict(new ErrorMessage("User had already started another game."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting game with ID: {id}",
                gameId);

            return StatusCode(500, new ErrorMessage($"Internal server error: {ex.Message}"));
        }
    }


    /// <summary>
    /// Execute player's command
    /// </summary>
    /// <param name="action">User action</param>
    /// <returns></returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/actions
    /// </remarks>
    /// <response code="200">Success.</response>
    /// <response code="400">Invalid data or user didn't start the game.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("actions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult UserInput([FromBody] UserAction action)
    {
        _logger.LogInformation("Request with user action: {id}, Hold: {hold}",
            action.ActionId, action.Hold);

        try
        {
            if (_model is null)
            {
                _logger.LogWarning("User didn't start the game.");

                return BadRequest(new ErrorMessage($"User didn't start the game."));
            }

            if (!Enum.IsDefined(typeof(Common.Enums.UserAction), action.ActionId))
            {
                _logger.LogWarning("Invalid action: {id}",
                    action.ActionId);

                return BadRequest(new ErrorMessage($"Invalid action: {action.ActionId}"));
            }

            _model.UserInput((Common.Enums.UserAction)action.ActionId, action.Hold);

            _logger.LogInformation("Successful user action: {id}, Hold: {hold}",
                action.ActionId, action.Hold);

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error with user action: {id}, Hold: {hold}",
                action.ActionId, action.Hold);

            return StatusCode(500, new ErrorMessage($"Internal server error: {ex.Message}"));
        }
    }


    /// <summary>
    /// Get current game state
    /// </summary>
    /// <returns>Current game state</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/state
    /// </remarks>
    /// <response code="200">Success.</response>
    /// <response code="400">User didn't start the game.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("state")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<GameState> UpdateCurrentState()
    {
        _logger.LogInformation("Request to get current game state");

        try
        {
            if (_model is null)
            {
                return BadRequest(new ErrorMessage($"User didn't start the game."));
            }

            var state = _model.UpdateCurrentState();

            return new GameState(state);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current game state: {message}",
                ex.Message);

            return StatusCode(500, new ErrorMessage($"Internal server error: {ex.Message}"));
        }
    }
}
