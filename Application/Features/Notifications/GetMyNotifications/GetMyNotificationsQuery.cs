using Application.DTOs.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Notifications.GetMyNotifications
{
    public class GetMyNotificationsQuery : IRequest<List<NotificationDto>> { }
}
