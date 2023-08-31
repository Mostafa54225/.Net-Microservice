using CommandService.Data;
using CommandService.IRepository;
using CommandService.Models;
using Microsoft.AspNetCore.Identity;

namespace CommandService.Repository
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }
        public void CreateCommand(int platformId, Command command)
        {
            if(command == null) throw new ArgumentNullException(nameof(command));
            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if(platform == null) throw new ArgumentNullException(nameof(platform));
            _context.Platforms.Add(platform);
        }

        public bool ExternalPlatformExist(int externalPlatformId)
        {
            return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _context.Commands.Where(c => c.PlatformId == platformId && c.Id == commandId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands.Where(c => c.PlatformId == platformId).OrderBy(c => c.Platform.Name);
        }

        public IEnumerable<Platform> GetPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public bool PlatformExist(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}