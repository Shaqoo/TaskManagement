using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tasks.CreateTask
{
    using Application.Abstractions;
    using Application.DTOs;
    using Application.DTOs.Tasks;
    using Domain.DomainEvents;
    using Domain.Entities;
    using MediatR;
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result<Guid>>
    {
        private readonly ITaskRepository _taskRepo;
        private readonly INotificationRepository _notifRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _user;
        private readonly INotifier _notifier;
        private readonly IMemoryCacheService _cache;

        public CreateTaskCommandHandler(ITaskRepository taskRepo,
            INotificationRepository notifRepo,
            IUnitOfWork uow,
            ICurrentUserService user,
            INotifier notifier,
            IMemoryCacheService cache)
        {
            _taskRepo = taskRepo;
            _notifRepo = notifRepo;
            _uow = uow;
            _user = user;
            _notifier = notifier;
            _cache = cache;
        }

        public async Task<Result<Guid>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                UserId = _user.UserId
            };

            await _taskRepo.AddAsync(task);
            await _uow.SaveChangesAsync();

            await _cache.RemoveAsync($"tasks:user:{_user.UserId}");

            var notif = new Notification
            {
                UserId = _user.UserId,
                Message = $"Your task '{task.Title}' was created.",
                CreatedAt = DateTime.UtcNow
            };

            await _notifRepo.AddAsync(notif);
            await _uow.SaveChangesAsync();

            await _notifier.SendNotificationAsync(_user.UserId, notif.Message);

            return Result<Guid>.Success(task.Id);
        }
    }

}
