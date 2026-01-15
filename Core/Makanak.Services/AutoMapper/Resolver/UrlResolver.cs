using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Services.AutoMapper.Resolver
{
    public class UrlResolver<TSource , TDestination>(IConfiguration configuration) : IMemberValueResolver<TSource, TDestination, string , string>
    {
        public string? Resolve(TSource source, TDestination destination, string sourceMember, string destMember, ResolutionContext context)
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
