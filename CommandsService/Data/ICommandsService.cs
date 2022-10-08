﻿using System.Collections.Generic;
using CommandsService.Models;

namespace CommandsService.Data
{
    public interface ICommandsService
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();

        void CreatePlatform(Platform plat);

        bool PlatformExists(int platformId);

        bool ExternalPlatformExist(int externalPlatformId);

        IEnumerable<Command> GetCommandsForPlatform(int platformId);

        Command GetCommand(int platformId, int commandId);

        void CreateCommand(int platformId, Command command);

    }
}
