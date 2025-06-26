using Application.DTOs;
using Application.DTOs.Tasks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tasks.GetMyTasks
{
    public class GetMyTasksQuery : IRequest<PaginatedResult<TaskDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
