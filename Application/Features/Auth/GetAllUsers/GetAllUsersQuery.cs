using Application.DTOs;
using Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.GetAllUsers
{
    public class GetAllUsersQuery : IRequest<PaginatedResult<UserDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
