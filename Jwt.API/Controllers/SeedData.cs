using Jwt.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SeedData : ControllerBase
    {
        private readonly AppDbInit _seeder;

        public SeedData(AppDbInit seeder)
        {
            _seeder = seeder;
        }
        [HttpPost("Seed Database")]
        public ActionResult Seed()
        {
            _seeder.Seed();

        }
    }
}
