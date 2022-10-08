using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandDataClient _commandDataClient;
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(ICommandDataClient commandDataClient,
            IPlatformRepo repo,
            IMapper mapper,
            IMessageBusClient messageBusClient)
        {
            _commandDataClient = commandDataClient;
            _repo = repo;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatform()
        {
            Console.WriteLine(".. Getting Platforms ..");
            var platformItems = await _repo.GetAllPlatforms();


            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
        {
            var platformItem = await  _repo.GetPlatformById(id);

            if (platformItem != null)
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreate)
        {
            var platformModal = _mapper.Map<Platform>(platformCreate);

             _repo.CreatePlatform(platformModal);
            await _repo.SaveChanges();
            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModal);
            // Sync message sending method
            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"-- A problem occurs during sending {ex.Message}--");
            }


            // Async message sending method
            try
            {
                var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);

                platformPublishedDto.Event = "Platform_Published";

                _messageBusClient.PublishNewPlatform(platformPublishedDto);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"-- A problem occurs during sending asynchronously {ex.Message}--");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}
