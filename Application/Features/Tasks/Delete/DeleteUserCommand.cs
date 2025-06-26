using Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tasks.Delete
{
    public class DeleteTaskCommand : IRequest<Result<string>>
    {
        public Guid TaskId { get; set; }
    }
}
