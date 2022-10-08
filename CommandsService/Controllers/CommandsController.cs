﻿using System;
using System.Collections.Generic;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [ApiController]
    [Route("api/c/platforms/{platformId}/[controller]")]
       
    public class CommandsController : Controller
      {
        private readonly ICommandsService _repo;
        private readonly IMapper _mapper;

        public CommandsController(ICommandsService repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlaform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatform {platformId} <--");

            if (!_repo.PlatformExists(platformId)) return NotFound();

            var commands = _repo.GetCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));

        }

        [HttpGet("{commandId}", Name ="GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform (int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit GetCommandForPlatform {platformId}/{commandId}<--");

            if (!_repo.PlatformExists(platformId)) return NotFound();

            var command = _repo.GetCommand(platformId, commandId);

            if (command == null)
                return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDtos commandCreateDto)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform {platformId}<--");

            if (!_repo.PlatformExists(platformId)) return NotFound();

            var command = _mapper.Map<Command>(commandCreateDto);

            _repo.CreateCommand(platformId, command);

            _repo.SaveChanges();
            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform),
                new { platformId, commandId = commandReadDto.Id }, commandReadDto);




        }


       
}
}
