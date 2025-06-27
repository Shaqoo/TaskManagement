using Application.Abstractions;
using Application.DTOs;
using Application.Features.Tasks.CreateTask;
using Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Handlers.Tasks
{
    public class CreateTaskCommandHandlerTests
    {
        private readonly Mock<ITaskRepository> _taskRepoMock;
        private readonly Mock<ICurrentUserService> _userMock;
        private readonly Mock<INotificationRepository> _notifRepoMock;
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly Mock<IMemoryCacheService> _cacheMock;
        private readonly Mock<INotifier> _notifierMock;
        private readonly CreateTaskCommandHandler _handler;

        private readonly Guid _testUserId = Guid.NewGuid();

        public CreateTaskCommandHandlerTests()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _userMock = new Mock<ICurrentUserService>();
            _notifRepoMock = new Mock<INotificationRepository>();
            _uowMock = new Mock<IUnitOfWork>();
            _cacheMock = new Mock<IMemoryCacheService>();
            _notifierMock = new Mock<INotifier>();

            _userMock.Setup(u => u.UserId).Returns(_testUserId);

            _handler = new CreateTaskCommandHandler(
                _taskRepoMock.Object,
                _notifRepoMock.Object,
                _uowMock.Object,
                _userMock.Object,
                _notifierMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndPerformsAllActions()
        {
            
            var command = new CreateTaskCommand
            {
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

             
            var result = await _handler.Handle(command, CancellationToken.None);

             
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();
            result.Data.Should().NotBeEmpty();

            _taskRepoMock.Verify(r => r.AddAsync(It.Is<TaskItem>(
                t => t.Title == command.Title &&
                     t.Description == command.Description &&
                     t.UserId == _testUserId
            )), Times.Once);

            _uowMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Exactly(1));

            _cacheMock.Verify(c => c.RemoveAsync($"tasks:user:{_testUserId}"), Times.Once);

            _notifRepoMock.Verify(n => n.AddAsync(It.Is<Notification>(
                notif => notif.UserId == _testUserId &&
                         notif.Message.Contains(command.Title)
            )), Times.Once);

            _notifierMock.Verify(n => n.SendNotificationAsync(
                _testUserId,
                It.Is<string>(msg => msg.Contains(command.Title))
            ), Times.Once);
        }
    }
}
