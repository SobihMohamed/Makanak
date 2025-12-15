using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.Resolver
{
    public class UrlResolver(IConfiguration configuration) : IMemberValueResolver<object, object,string , string>
    {
        public string? Resolve(object source, object destination, string sourceMember, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(sourceMember))
            {
                return sourceMember;
            }
            var baseUrl = configuration["Urls:BaseUrl"];
            return $"{baseUrl}{sourceMember}";
        }
    }
}
