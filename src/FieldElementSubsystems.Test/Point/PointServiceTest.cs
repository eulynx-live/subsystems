using EulynxLive.FieldElementSubsystems.Interfaces;
using EulynxLive.Point;
using EulynxLive.Point.Proto;
using EulynxLive.Point.Services;

using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Moq;

namespace FieldElementSubsystems.Test;

public class PointServiceTest()
{
    [Fact]
    public void TestReset()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        pointService.Reset(new Empty(), Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.Reset(), Times.Once);
    }

    [Fact]
    public void TestScheduleTimeoutRight()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        pointService.ScheduleTimeoutRight(new EnableMovementFailedMessage() { EnableMovementFailed = true }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.EnableTimeoutRight(true), Times.Once);
    }

    [Fact]
    public void TestScheduleTimeoutLeft()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        pointService.ScheduleTimeoutLeft(new EnableMovementFailedMessage() { EnableMovementFailed = true }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.EnableTimeoutLeft(true), Times.Once);
    }

    [Fact]
    public async Task TestSetAbilityToMove()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        await pointService.SetAbilityToMove(new AbilityToMoveMessage() { Ability = AbilityToMove.AbleToMove }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.SetAbilityToMove(It.IsAny<AbilityToMoveMessage>()), Times.Once);
    }

    [Fact]
    public async Task TestSendSciMessage()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        await pointService.SendSciMessage(new SciMessage() { Message = ByteString.CopyFrom(new byte[] { 0x01 }) }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.SendSciMessage(It.IsAny<SciMessage>()), Times.Once);
    }

    [Fact]
    public async Task TestOverrideSciMessage()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.Connection.OverrideNextSciMessage(It.IsAny<byte[]>()))
            .Returns(Task.FromResult(0));
        var pointService = new PointService(point.Object);

        // Act
        await pointService.OverrideSciMessage(new SciMessage() { Message = ByteString.CopyFrom(new byte[] { 0x01 }) }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.Connection.OverrideNextSciMessage(It.IsAny<byte[]>()), Times.Once);
    }

    [Fact]
    public void TestSchedulePreventLeftEndPosition()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        pointService.SchedulePreventLeftEndPosition(new PreventedPositionMessage() { Position = PreventedPosition.SetNoEndPosition, DegradedPosition = true }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.PreventLeftEndPosition(It.IsAny<PreventedPositionMessage>()), Times.Once);
    }

    [Fact]
    public void TestSchedulePreventRightEndPosition()
    {
        // Arrange
        var point = new Mock<IPoint>();
        var pointService = new PointService(point.Object);

        // Act
        pointService.SchedulePreventRightEndPosition(new PreventedPositionMessage() { Position = PreventedPosition.SetNoEndPosition, DegradedPosition = true }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.PreventRightEndPosition(It.IsAny<PreventedPositionMessage>()), Times.Once);
    }

    [Fact]
    public async Task TestPutIntoTrailedPosition()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.ConnectionProtocol)
            .Returns(EulynxLive.FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R1);
        point.Setup(x => x.PointState)
            .Returns(new GenericPointState(LastCommandedPointPosition: null,
                PointPosition: GenericPointPosition.Left,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ));
        var pointService = new PointService(point.Object);

        // Act
        await pointService.PutIntoTrailedPosition(new DegradedPositionMessage() { DegradedPosition = true }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.PutIntoUnintendedPosition(It.IsAny<DegradedPositionMessage>()), Times.Once);
    }

    [Fact]
    public async Task TestPutIntoTrailedPositionThrowsIfNotSupported()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.ConnectionProtocol)
            .Returns(EulynxLive.FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R2);
        point.Setup(x => x.PointState)
            .Returns(new GenericPointState(LastCommandedPointPosition: null,
                PointPosition: GenericPointPosition.Left,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ));
        var pointService = new PointService(point.Object);

        // Assert
        await Assert.ThrowsAsync<RpcException>(() => pointService.PutIntoTrailedPosition(new DegradedPositionMessage() { DegradedPosition = true }, Mock.Of<ServerCallContext>()));
    }

    [Fact]
    public async Task TestPutIntoUnintendedPosition()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.ConnectionProtocol)
            .Returns(EulynxLive.FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R2);
        var pointService = new PointService(point.Object);

        // Act
        await pointService.PutIntoUnintendedPosition(new DegradedPositionMessage() { DegradedPosition = true }, Mock.Of<ServerCallContext>());

        // Assert
        point.Verify(x => x.PutIntoUnintendedPosition(It.IsAny<DegradedPositionMessage>()), Times.Once);
    }

    [Fact]
    public async Task TestPutIntoUnintendedPositionThrowsIfNotSupported()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.ConnectionProtocol)
            .Returns(EulynxLive.FieldElementSubsystems.Configuration.ConnectionProtocol.EulynxBaseline4R1);
        var pointService = new PointService(point.Object);

        // Assert
        await Assert.ThrowsAsync<RpcException>(() => pointService.PutIntoUnintendedPosition(new DegradedPositionMessage() { DegradedPosition = true }, Mock.Of<ServerCallContext>()));
    }

    [Fact]
    public async Task TestGetDegradedPointPosition()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.PointState)
            .Returns(new GenericPointState(LastCommandedPointPosition: null,
                PointPosition: GenericPointPosition.Left,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ));
        var pointService = new PointService(point.Object);

        // Act
        var result = await pointService.GetDegradedPointPosition(new Empty(), Mock.Of<ServerCallContext>());

        // Assert
        Assert.Equal(PointDegradedPosition.NotApplicable, result.Position);
    }

    [Fact]
    public async Task TestGetPointPosition()
    {
        // Arrange
        var point = new Mock<IPoint>();
        point.Setup(x => x.PointState)
            .Returns(new GenericPointState(LastCommandedPointPosition: null,
                PointPosition: GenericPointPosition.Left,
                DegradedPointPosition: GenericDegradedPointPosition.NotApplicable,
                AbilityToMove: GenericAbilityToMove.AbleToMove
            ));
        var pointService = new PointService(point.Object);

        // Act
        var result = await pointService.GetPointPosition(new Empty(), Mock.Of<ServerCallContext>());

        // Assert
        Assert.Equal(PointPosition.Left, result.Position);
    }
}
