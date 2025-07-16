using Xunit;
using SpaceLogic.Data.Models.Admin;
using backend.Tests.Shared;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests.Admin
{
    public class AdminDbContextTests
    {
        [Fact]
        public async Task CanAddUserToAdminDbContext()
        {
            // Arrange
            var db = Helpers.GetInMemoryAdminDbContext("AdminTestDb");

            var orgId = Guid.NewGuid();

            var user = new User
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Email = "ada@math.com",
                Password = "secure123",
                OrganizationId = orgId,
                CreatedAt = DateTime.UtcNow,
                LastActive = DateTime.UtcNow,
                IsActive = true
            };

            // Act
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            // Assert
            var count = await db.Users.CountAsync();
            Assert.Equal(1, count);
        }
    }
}
