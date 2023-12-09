using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase

    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public PlatformController(IPlatformRepository platformRepository, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> getPlatforms() 
        {
            Console.WriteLine("--> Geting Platforms...");

            var platforms = _platformRepository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
        }

        [HttpGet("{id}", Name = "getPlatformbyId")]
        public ActionResult<PlatformReadDto> getPlatformbyId(int id)
        {
            Console.WriteLine("--> Geting Platform by Id...");

            var platform = _platformRepository.GetPlatformById(id);

            if (platform == null) { return NotFound(); }

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost()]
        public ActionResult<PlatformReadDto> CreatePlatform([FromBody] PlatformCreateDto platform)
        {
            Console.WriteLine("--> Adding Platform...");

            var platformModel = _mapper.Map<Platform>(platform);

            _platformRepository.CreatePlatform(platformModel);

            _platformRepository.SaveChanges();

            var platformRead = _mapper.Map<PlatformReadDto>(platformModel);

            return CreatedAtRoute(nameof(getPlatformbyId), new {Id= platformRead.Id}, platformRead);

 

        }



    }
}
