using Microsoft.AspNetCore.Mvc;
using WeatherHistory.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WeatherHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public List<User> Get()
        {
            return DatabaseHelper.GetUsers();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public User Get(long id)
        {
            return DatabaseHelper.GetUserById(id);
        }

        // POST api/<UserController>
        [HttpPost]
        public void Post([FromBody] User user)
        {
            DatabaseHelper.SaveUser(user.UserId,user.FirstName,user.LastName);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public User Put(long id, [FromQuery] string firstName, [FromQuery] string lastName)
        {
            return DatabaseHelper.UpdateUser(id,firstName,lastName);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
            DatabaseHelper.DeleteUser(id);
        }
    }
}
