using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.GetMyNotifications
{
    using Application.Abstractions;
    using Application.DTOs.Notifications;
    using MediatR;
    public class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _repo;
        private readonly ICurrentUserService _user;

        public GetMyNotificationsHandler(INotificationRepository repo, ICurrentUserService user)
        {
            _repo = repo;
            _user = user;
        }

        public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notes = await _repo.GetUnreadByUserIdAsync(_user.UserId);
            return notes.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                CreatedAt = n.CreatedAt
            }).ToList();
        }
    }


}
