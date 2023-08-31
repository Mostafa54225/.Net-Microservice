using System.Text.Json;
using AutoMapper;
using CommandService.Dtos;
using CommandService.IRepository;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch(eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;

            }
        }

        private static EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event.");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }
        private void AddPlatform(string platformPublisedMessage)
        {
            using(var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublisedMessage);
                try
                {
                    var plat = _mapper.Map<PlatformPublishDto, Platform>(platformPublishedDto);
                    Platform newPlatform = new ()
                    {
                        ExternalId = platformPublishedDto.Id,
                        Name = platformPublishedDto.Name
                    };
                    if(!repo.ExternalPlatformExist(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    } else
                    {
                        Console.WriteLine("--> Platform already exist");
                    }
                } catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not add platform to db {ex.Message}");                    
                }
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}