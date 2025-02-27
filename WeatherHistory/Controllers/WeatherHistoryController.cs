using Microsoft.AspNetCore.Mvc;
using WeatherHistory.Models;

namespace WeatherHistory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherHistoryController : ControllerBase
    {
        // GET: api/<WeatherHistoryController>
        [HttpGet]
        public List<Models.WeatherHistory> Get()
        {
            return DatabaseHelper.GetWeathers();
        }

        // GET api/<WeatherHistoryController>/5
        [HttpGet("{id}")]
        public Models.WeatherHistory Get(int id)
        {
            return DatabaseHelper.GetWeatherById(id);
        }

        // POST api/<WeatherHistoryController>
        [HttpPost]
        public void Post([FromQuery] long userId, [FromQuery] string city, [FromQuery] decimal temp,
                                                   [FromQuery] decimal humid, [FromQuery] decimal wind, [FromQuery] string condition)
        {
             DatabaseHelper.SaveWeather(userId, city, temp, humid, wind, condition);
        }

        // PUT api/<WeatherHistoryController>/5
        [HttpPut("{id}")]
        public Models.WeatherHistory Put(int id, [FromQuery] long userId, [FromQuery] string city, [FromQuery] decimal temp,
                                                   [FromQuery] decimal humid, [FromQuery] decimal wind, [FromQuery] string condition)
        {
            return DatabaseHelper.UpdateWeatherHistory(id,userId, city, temp, humid, wind, condition);
        }

        // DELETE api/<WeatherHistoryController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            DatabaseHelper.DeleteWeatherHistory(id);
        }

        // GET api/weatherhistory/user/5
        [HttpGet("user/{userId}")]
        public List<Models.WeatherHistory> GetByUserId(long userId)
        {
            return DatabaseHelper.GetAllFromHistory(userId);
        }

        [HttpPost("/sendWeatherToAll")]
        public void PostToAllUsers([FromQuery] string city)
        {
            List<User> users = DatabaseHelper.GetUsers();

            foreach (User user in users)
            {
                TelegramBotService.SendWeatherToUser(user, city);
            }

        }

        [HttpPost("/sendWeatherToUser/{userId}")]
        public void PostToUser(long userId, [FromQuery] string city)
        {
            User user = DatabaseHelper.GetUserById(userId);

            TelegramBotService.SendWeatherToUser(user, city);
        }

    }
}
