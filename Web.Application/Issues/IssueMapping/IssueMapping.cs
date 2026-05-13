using AutoMapper;
using Web.Application.Issues.DTOs;
using Web.Domain.Entities;

namespace Web.Application.Issues.IssueMapping
{
    public class IssueMapping : Profile
    {
        public IssueMapping()
        {
            CreateMap<Issue, IssueDto>();
        }
    }
}
