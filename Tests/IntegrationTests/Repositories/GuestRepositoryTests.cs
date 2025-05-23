using SqliteDataWrite.UnitOfWork;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate;
using VIAEventAssociation.Core.Domain.Aggregates.GuestAggregate.ValueObjects;
using VIAEventAssociation.Core.Domain.Common.UnitOfWork;
using VIAEventAssociation.Infrastructure.SqliteDataWrite;
using VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories.GuestRepository;
using IGuestRepository = VIAEventAssociation.Infrastructure.SqliteDataWrite.Repositories.GuestRepository.IGuestRepository;

namespace IntegrationTests.Repositories;

public class GuestRepositoryTests : IDisposable
{
    private readonly WriteDbContext _context;
    private readonly IGuestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public GuestRepositoryTests()
    {
        // Use the helper method to set up the context
        _context = WriteDbContext.SetupContext();
        
        // Create repository and unit of work
        _repository = new GuestRepository(_context);
        _unitOfWork = new SqliteUnitOfWork(_context);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
    
    [Fact]
    public async Task AddAndGetGuest_ShouldReturnSameGuest()
    {
        // Arrange
        var guestId = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("John").Value;
        var lastName = LastName.Create("Doe").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePic = ProfilePictureUrl.Create("https://example.com/profile.jpg").Value;
        
        var guest = Guest.Create(guestId, firstName, lastName, email, profilePic).Value;
        
        // Act
        await _repository.AddAsync(guest);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker
        _context.ChangeTracker.Clear();
        
        var retrievedGuest = await _repository.GetAsync(guestId);
        
        // Assert
        Assert.NotNull(retrievedGuest);
        Assert.Equal(guestId.Value, retrievedGuest.Id.Value);
        Assert.Equal(firstName.Value, retrievedGuest.firstName.Value);
        Assert.Equal(lastName.Value, retrievedGuest.lastName.Value);
        Assert.Equal(email.Value, retrievedGuest.email.Value);
        Assert.Equal(profilePic.Value, retrievedGuest.profilePictureUrl.Value);
    }
    
    [Fact]
    public async Task UpdateGuest_ShouldPersistChanges()
    {
        // Arrange
        var guestId = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName = FirstName.Create("John").Value;
        var lastName = LastName.Create("Doe").Value;
        var email = ViaEmail.Create("331458@via.dk").Value;
        var profilePic = ProfilePictureUrl.Create("https://example.com/profile.jpg").Value;
        
        var guest = Guest.Create(guestId, firstName, lastName, email, profilePic).Value;
        
        await _repository.AddAsync(guest);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker
        _context.ChangeTracker.Clear();
        
        // Get the guest first to ensure we're working with a tracked entity
        var retrievedGuest = await _repository.GetAsync(guestId);
        
        // Act - Update the guest
        var newFirstName = FirstName.Create("Jane").Value;
        retrievedGuest.UpdateFirstName(newFirstName);
        
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker again
        _context.ChangeTracker.Clear();
        
        // Get the guest again
        var updatedGuest = await _repository.GetAsync(guestId);
        
        // Assert
        Assert.Equal(newFirstName.Value, updatedGuest.firstName.Value);
    }
    
    [Fact]
    public async Task GetByEmail_ShouldReturnCorrectGuest()
    {
        // Arrange
        var guestId1 = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName1 = FirstName.Create("John").Value;
        var lastName1 = LastName.Create("Doe").Value;
        var email1 = ViaEmail.Create("331458@via.dk").Value;
        var profilePic1 = ProfilePictureUrl.Create("https://example.com/profile1.jpg").Value;
        
        var guest1 = Guest.Create(guestId1, firstName1, lastName1, email1, profilePic1).Value;
        
        var guestId2 = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName2 = FirstName.Create("Jane").Value;
        var lastName2 = LastName.Create("Smith").Value;
        var email2 = ViaEmail.Create("123456@via.dk").Value;
        var profilePic2 = ProfilePictureUrl.Create("https://example.com/profile2.jpg").Value;
        
        var guest2 = Guest.Create(guestId2, firstName2, lastName2, email2, profilePic2).Value;
        
        await _repository.AddAsync(guest1);
        await _repository.AddAsync(guest2);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker
        _context.ChangeTracker.Clear();
        
        // Act
        var retrievedGuest = await _repository.GetByEmailAsync(email1);
        
        // Assert
        Assert.NotNull(retrievedGuest);
        Assert.Equal(guestId1.Value, retrievedGuest.Id.Value);
        Assert.Equal(email1.Value, retrievedGuest.email.Value);
    }
    
    [Fact]
    public async Task GetAll_ShouldReturnAllGuests()
    {
        // Arrange
        var guestId1 = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName1 = FirstName.Create("John").Value;
        var lastName1 = LastName.Create("Doe").Value;
        var email1 = ViaEmail.Create("331458@via.dk").Value;
        var profilePic1 = ProfilePictureUrl.Create("https://example.com/profile1.jpg").Value;
        
        var guest1 = Guest.Create(guestId1, firstName1, lastName1, email1, profilePic1).Value;
        
        var guestId2 = GuestId.FromGuid(Guid.NewGuid()).Value;
        var firstName2 = FirstName.Create("Jane").Value;
        var lastName2 = LastName.Create("Smith").Value;
        var email2 = ViaEmail.Create("123456@via.dk").Value;
        var profilePic2 = ProfilePictureUrl.Create("https://example.com/profile2.jpg").Value;
        
        var guest2 = Guest.Create(guestId2, firstName2, lastName2, email2, profilePic2).Value;
        
        await _repository.AddAsync(guest1);
        await _repository.AddAsync(guest2);
        await _unitOfWork.SaveChangesAsync();
        
        // Clear change tracker
        _context.ChangeTracker.Clear();
        
        // Act
        var guests = await _repository.GetAllAsync();
        
        // Assert
        var guestsList = guests.ToList();
        Assert.Equal(2, guestsList.Count);
        Assert.Contains(guestsList, g => g.Id.Value == guestId1.Value);
        Assert.Contains(guestsList, g => g.Id.Value == guestId2.Value);
    }
}