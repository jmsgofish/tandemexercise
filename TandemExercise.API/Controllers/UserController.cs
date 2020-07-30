using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TandemExercise.Business.Entities;
using TandemExercise.Business.Entities.DTO;
using TandemExercise.Data;
using TandemExercise.Data.Exceptions;

namespace TandemExercise.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IUserRepository userRepository;
        private readonly ILogger logger;

        public UserController(IMapper mapper, IUserRepository userRepository, ILogger<UserController> logger)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.logger = logger;
        }

        // GET api/<UserController>
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            logger.LogInformation("Request to get all users");
            return await this.userRepository.Get();
        }

        // GET api/<UserController>/emailAddress
        [HttpGet("{emailAddress}")]
        public async Task<IActionResult> Get(string emailAddress)
        {
            logger.LogInformation($"Request to retrieve email {emailAddress}");
            var user = await this.userRepository.Get(emailAddress);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<UserDTO>(user));
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<User>> Post([FromBody] User json)
        {
            logger.LogInformation($"post to create user {JsonConvert.SerializeObject(json)}");
            User user = null;

            try
            {
                user = await this.userRepository.Create(json);
            }
            catch (DuplicateEmailException)
            {
                return Conflict();
            }

            logger.LogInformation($"created user with id: {user.id}");
            return Ok(user);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Put(string id, [FromBody] User json)
        {
            logger.LogInformation($"put to update user {JsonConvert.SerializeObject(json)}");
            User user = null;

            try
            {
                user = await this.userRepository.Update(json);
            }
            catch (DuplicateEmailException)
            {
                return Conflict();
            }

            return Ok(user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
