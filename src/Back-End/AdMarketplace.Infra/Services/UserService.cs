using AdMarketplace.Database;
using AdMarketplace.Database.Models;
using AdMarketplace.Domain.Contracts.Common;
using AdMarketplace.Infra.Interfaces;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace AdMarketplace.Infra.Services;

public class UserService(AdMarketDbContext dbContext) : IUserService
{
     public async Task<ErrorOr<User>> EnsureExistsAsync(InitDataUserContract initData)
     {
         var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserId.Equals(initData.Id));
         if (user is not null)
             return user;

         var userToCreate = User.Create(
             userId: initData.Id,
             firstName: initData.FirstName,
             lastName: initData.LastName,
             userName: initData.UserName
         );

         dbContext.Users.Add(userToCreate);
         
         await dbContext.SaveChangesAsync();

         return userToCreate;
     }
}