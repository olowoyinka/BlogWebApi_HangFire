using BlogWebApi.Application.Interfaces.Services;
using BlogWebApi.Contracts.Commons;
using BlogWebApi.Contracts.DTOs.Requests;
using BlogWebApi.Contracts.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace BlogWebApi.API.Controllers
{
    [Authorize(Policy = "AllowAccess")]
    public class BlogsController : BaseApiController
    {
        private readonly IBlogService BlogService;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(IBlogService blogService,
                                ILogger<BlogsController> logger)
        {
            this.BlogService = blogService;
            this._logger = logger;
        }


        [HttpGet("blogs")]
        [ProducesResponseType(typeof(DataResponseArrayDTO<BlogResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult GetAllBlog([FromQuery] int page = 1,
                                                     [FromQuery] int size = 10,
                                                     [FromQuery] string title = "",
                                                     [FromQuery] string email = "")
        {
            try
            {
                _logger.LogInformation("BlogsController GetAllBlog method called");

                var dd = GetUserId();

                var response = this.BlogService.GetAllBlog(page, size, title, email);

                return Ok(new DataResponseArrayDTO<BlogResponseDTO>(response.getAllBlog, response.totalCount, page, size));
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
                _logger.LogError("Error occur while getting all blog");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }
        


        [HttpPost("blogs")]
        [Authorize(Policy = "CreatorAccess")]
        [ProducesResponseType(typeof(DataResponseDTO<BlogResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult CreateBlog([FromBody] BlogRequestDTO model)
        {
            try
            {
                _logger.LogInformation("BlogsController CreateBlog method called");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }

                var response = this.BlogService.CreateBlog(GetUserId(), model);

                return Ok(new DataResponseDTO<BlogResponseDTO>(response));
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
                _logger.LogError("Error occur while creating blog");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        [HttpGet("blogs/{id}")]
        [ProducesResponseType(typeof(DataResponseDTO<BlogResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult GetBlogById([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("BlogsController GetBlogById method called");

                var response = this.BlogService.GetBlog(id);

                return Ok(new DataResponseDTO<BlogResponseDTO>(response));
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
                _logger.LogError("Error occur while getting a blog by id");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        [HttpPatch("blogs/{id}")]
        [Authorize(Policy = "CreatorAccess")]
        [ProducesResponseType(typeof(DataResponseDTO<BlogResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult UpdateBlog([FromRoute] int id, [FromBody] BlogUpdateDTO model)
        {
            try
            {
                _logger.LogInformation("BlogsController UpdateBlog method called");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }

                var response = this.BlogService.UpdateBlog(GetUserId(), id, model);

                return Ok(new DataResponseDTO<BlogResponseDTO>(response));
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
                _logger.LogError("Error occur while updating blog by id");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        [HttpDelete("blogs/{id}")]
        [Authorize(Policy = "CreatorAccess")]
        [ProducesResponseType(typeof(DataResponseDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult DeleteBlog([FromRoute] int id)
        {
            try
            {
                _logger.LogInformation("BlogsController DeleteBlog method called");

                var response = this.BlogService.DeleteBlog(GetUserId(), id);

                return Ok(new DataResponseDTO<string>(response));
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
                _logger.LogError("Error occur while delete blog by id");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        [HttpPost("blogs/{id}/comment")]
        [ProducesResponseType(typeof(DataResponseDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult CreateBlogComment([FromRoute] int id, [FromBody] CommentRequestDTO model)
        {
            try
            {
                _logger.LogInformation("BlogsController CreateBlogComment method called");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }

                var response = this.BlogService.CreateComment(GetUserId(), id, model);

                return Ok(new DataResponseDTO<string>(response));
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
                _logger.LogError("Error occur while creating blog comment");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        [HttpPatch("blogs/{id}/comment/{commentId}")]
        [ProducesResponseType(typeof(DataResponseDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult UpdateBlogComment([FromRoute] int id, [FromRoute] int commentId, [FromBody] CommentUpdateDTO model)
        {
            try
            {
                _logger.LogInformation("BlogsController UpdateBlogComment method called");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ModelStateErrorResponseDTO(HttpStatusCode.BadRequest,
                        ModelState));
                }

                var response = this.BlogService.UpdateComment(GetUserId(), id, commentId, model);

                return Ok(new DataResponseDTO<string>(response));
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
                _logger.LogError("Error while updating blog comment");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        [HttpDelete("blogs/{id}/comment/{commentId}")]
        [ProducesResponseType(typeof(DataResponseDTO<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponseDTO), StatusCodes.Status404NotFound)]
        public IActionResult DeleteBlogComment([FromRoute] int id, [FromRoute] int commentId)
        {
            try
            {
                _logger.LogInformation("BlogsController DeleteBlogComment method called");

                var response = this.BlogService.DeleteComment(GetUserId(), id, commentId);

                return Ok(new DataResponseDTO<string>(response));
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
                _logger.LogError("Error occur while delete blog comment");

                return BadRequest(new ErrorResponseDTO(HttpStatusCode.BadRequest,
                                                              new string[] { ex.Message }));
            }
        }



        private string GetUserId()
        {
            return HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
    }
}