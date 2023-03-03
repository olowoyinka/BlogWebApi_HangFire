using BlogWebApi.Application.Interfaces.Services;
using BlogWebApi.Contracts.Commons;
using BlogWebApi.Contracts.DTOs.Requests;
using BlogWebApi.Contracts.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BlogWebApi.API.Controllers
{
    public class UsersController : BaseApiController
    {
        private readonly IUserService UserService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService,
                                ILogger<UsersController> logger)
        {
            this.UserService = userService;
            this._logger = logger;
        }


        [HttpPost("users/register")]
        [ProducesResponseType(typeof(DataResponseDTO<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Register ([FromBody] RegisterRequestDTO model)
        {
            try
            {
                _logger.LogInformation("UsersController Register method called");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }

                var response = await this.UserService.Register(model);

                return Ok(response);
            }
            catch (NotFoundException erx)
            {
                return NotFound(new ErrorResponseDTO(HttpStatusCode.NotFound,
                                                              new string[] { erx.Message }));
            }
            catch (BadRequestException erx)
            {
                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { erx.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occur in registering user");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }


        [HttpPost("users/login")]
        [ProducesResponseType(typeof(DataResponseDTO<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            try
            {
                _logger.LogInformation("UsersController Login method called");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }

                var response = await this.UserService.Login(model);

                return Ok(response);
            }
            catch (NotFoundException erx)
            {
                return NotFound(new ErrorResponseDTO(HttpStatusCode.NotFound,
                                                              new string[] { erx.Message }));
            }
            catch (BadRequestException erx)
            {
                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { erx.Message }));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occur in login user");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }
    }
}