using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformController : ControllerBase

    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly iCommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformController(IPlatformRepository platformRepository, IMapper mapper, iCommandDataClient commandDataClient, IMessageBusClient messageBusClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
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
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform([FromBody] PlatformCreateDto platform)
        {
            Console.WriteLine("--> Adding Platform...");

            var platformModel = _mapper.Map<Platform>(platform);

            _platformRepository.CreatePlatform(platformModel);

            _platformRepository.SaveChanges();

            var platformRead = _mapper.Map<PlatformReadDto>(platformModel);

            //Send Sync Message
            try 
            {
                await _commandDataClient.SendPlatformToCommand(platformRead);
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            //Send Async Message
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformRead);
                platformPublishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(platformPublishedDto);

            } catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send Asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(getPlatformbyId), new {Id= platformRead.Id}, platformRead);

 

        }



    }
}
