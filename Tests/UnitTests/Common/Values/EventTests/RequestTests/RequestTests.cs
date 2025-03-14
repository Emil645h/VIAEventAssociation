﻿using VIAEventAssociation.Core.Domain.Aggregates.Events.Request;
using VIAEventAssociation.Core.Domain.Aggregates.Events.Request.ValueObjects;
using VIAEventAssociation.Core.Tools.OperationResult.OperationResult;

namespace UnitTests.Common.Values.EventTests.RequestTests;

public class RequestTests
{
    [Fact]
    public void Create_WithAllValidData_ReturnsRequest()
    {
        // Arrange
        var idResult = RequestId.Create(Guid.NewGuid());
        var reason = "I'm super passionate about VIA.";

        var requestId = ((Success<RequestId>)idResult).Value;
        
        // Act
        var requestResult = Request.Create(requestId, reason);
        
        // Assert
        Assert.True(requestResult.IsSuccess);
        var success = Assert.IsType<Success<Request>>(requestResult);
        var request = success.Value;
        Assert.Equal(requestId, request.Id);
        Assert.Equal(reason, request.reason);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidReason_ReturnsError(string reason)
    {
        // Arrange
        var idResult = RequestId.Create(Guid.NewGuid());
        var requestId = ((Success<RequestId>)idResult).Value;
        
        // Act
        var requestResult = Request.Create(requestId, reason);
        
        // Assert
        Assert.True(requestResult.IsFailure);
        var resultFailure = Assert.IsType<Failure<Request>>(requestResult);
        Assert.Contains(resultFailure.Errors, e => e == RequestErrors.Reason.IsEmpty);
    }
}