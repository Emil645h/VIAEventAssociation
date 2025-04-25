using SqliteDataWrite;
using SqliteDataWrite.Repositories.EventRepository;
using SqliteDataWrite.UnitOfWork;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.EventAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Core.Domain.Common.Values;
using IEventRepository = SqliteDataWrite.Repositories.EventRepository.IEventRepository;

namespace IntegrationTests.Repositories;

public class EventRepositoryTests : IDisposable
{
    private readonly WriteDbContext _context;
    private readonly IEventRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public EventRepositoryTests()
    {
        // Use the helper method to set up the context
        _context = WriteDbContext.SetupContext();
        
        // Create repository and unit of work
        _repository = new EventRepository(_context);
        _unitOfWork = new SqliteUnitOfWork(_context);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
    
    [Fact]
    public async Task AddAndGetEvent_ShouldReturnSameEvent()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var event1 = Event.Create(eventId).Value;
        
        // Update with some data
        var title = EventTitle.Create("Test Event").Value;
        var description = EventDescription.Create("Test Description").Value;
        var currentTime = new ActualCurrentTime();
        var startTime = DateTime.Now.AddDays(1);
        var endTime = DateTime.Now.AddDays(1).AddHours(2);
        var eventTime = EventTime.Create(startTime, endTime, currentTime).Value;
        
        event1.UpdateTitle(title);
        event1.UpdateDescription(description);
        event1.UpdateTime(eventTime);
        event1.MakePublic();
        
        // Act
        await _repository.AddAsync(event1);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker to ensure we're getting a fresh read from the database
        _context.ChangeTracker.Clear();
        
        var retrievedEvent = await _repository.GetAsync(eventId);
        
        // Assert
        Assert.NotNull(retrievedEvent);
        Assert.Equal(eventId.Value, retrievedEvent.Id.Value);
        Assert.Equal(title.Value, retrievedEvent.title.Value);
        Assert.Equal(description.Value, retrievedEvent.description.Value);
        Assert.Equal(EventVisibility.Public.Value, retrievedEvent.visibility.Value);
    }
    
    [Fact]
    public async Task AddEventWithGuestList_ShouldRetrieveEventWithGuestList()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var event1 = Event.Create(eventId).Value;
        
        // Act
        await _repository.AddAsync(event1);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker
        _context.ChangeTracker.Clear();
        
        var retrievedEvent = await _repository.GetAsync(eventId);
        
        // Assert
        Assert.NotNull(retrievedEvent);
        Assert.NotNull(retrievedEvent.guestList);
        Assert.Equal(event1.guestList.Id.Value, retrievedEvent.guestList.Id.Value);
    }
    
    [Fact]
    public async Task DeleteEvent_ShouldRemoveEventFromDatabase()
    {
        // Arrange
        var eventId = EventId.Create(Guid.NewGuid()).Value;
        var event1 = Event.Create(eventId).Value;
        
        await _repository.AddAsync(event1);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker
        _context.ChangeTracker.Clear();
        
        // Act
        await _repository.RemoveAsync(eventId);
        await _unitOfWork.SaveChangesAsync();
        
        // Assert - should throw KeyNotFoundException
        await Assert.ThrowsAsync<KeyNotFoundException>(async () => 
            await _repository.GetAsync(eventId));
    }
}