using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tasks.GetMyTasks
{
    using Application.Abstractions;
    using Application.DTOs;
    using Application.DTOs.Auth;
    using Application.DTOs.Tasks;
    using Application.Features.Auth.GetAllUsers;
    using MediatR;
    public class GetMyTaskHandler : IRequestHandler<GetMyTasksQuery, PaginatedResult<TaskDto>>
    {
        private readonly ITaskRepository _repo;
        private readonly IMemoryCacheService _cache;
        private readonly ICurrentUserService _currentUser;

        public GetMyTaskHandler(ITaskRepository repo, IMemoryCacheService cache, ICurrentUserService currentUser)
        {
            _repo = repo;
            _cache = cache;
            _currentUser = currentUser;
        }

        public async Task<PaginatedResult<TaskDto>> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"users:{request.Page}:{request.PageSize}";
            var cached = await _cache.GetAsync<PaginatedResult<TaskDto>>(cacheKey);
            if (cached is not null) return cached;

            var (users, count) = await _repo.GetAllByUserAsync(_currentUser.UserId,request.Page, request.PageSize);
            var result = new PaginatedResult<TaskDto>
            {
                Items = users.Select(u => new TaskDto
                {
                    Id = u.Id,
                    Description = u.Description,
                    Title = u.Title,
                    DueDate = u.DueDate,
                    Status = u.Status.ToString(),
                    CreatedAt = u.CreatedAt
                }).ToList(),
                TotalCount = count,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };

            await _cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(3));
            return result;
        }
    }

}
