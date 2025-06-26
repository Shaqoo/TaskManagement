using Application.DTOs;
using Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.GetUser
{
    public class GetMyProfileQuery : IRequest<Result<UserDto>> { }
   
}
