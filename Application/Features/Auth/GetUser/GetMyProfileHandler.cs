using Application.Abstractions;
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
    public class GetMyProfileHandler(ICurrentUserService currentUserService,IUserRepository userRepository) : IRequestHandler<GetMyProfileQuery, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
             if (currentUserService == null)
             {
                 return  Result<UserDto>.Failure("User not authenticated.");
             }
             var user = await userRepository.GetByIdAsync(currentUserService.UserId);
             if (user == null)
             {
              return Result<UserDto>.Failure("User not found.");
             }
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    CreatedAt = user.CreatedAt
                };
                return Result<UserDto>.Success(userDto);

        }
    }
}
