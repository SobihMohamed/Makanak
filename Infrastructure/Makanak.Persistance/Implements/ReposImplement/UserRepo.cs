using Makanak.Domain.Contracts.Repos;
using Makanak.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Persistance.Implements.ReposImplement
{
    public class UserRepo(MakanakDbContext _context) : IUserRepository
    {
        public async Task<bool> IsUserNationalIdExistAsync(string nationalId, string excludedCurrentUserId)
        {
            return await _context.Users
                .AnyAsync(user => user.NationalId == nationalId && user.Id != excludedCurrentUserId);
        }
    }
}
