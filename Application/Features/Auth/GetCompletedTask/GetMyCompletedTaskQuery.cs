using Application.DTOs;
using Application.DTOs.Tasks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.GetCompletedTask
{
    public record GetMyCompletedTasksQuery : IRequest<PaginatedResult<TaskDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
