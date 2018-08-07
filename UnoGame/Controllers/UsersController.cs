﻿using System.Net;
using System.Net.Http;
using System.Web.Http;
using UnoGame.Models;
using UnoGame.Services;

namespace UnoGame.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("add")]
        [HttpPost]
        public HttpResponseMessage AddUser(User user)
        {
            _userService.Add(user);
            return Request.CreateResponse(HttpStatusCode.OK, _userService.Users);
        }

        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetAll(User user)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _userService.Users);
        }
    }
}