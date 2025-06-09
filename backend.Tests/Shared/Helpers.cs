using Microsoft.EntityFrameworkCore;
using SpaceLogic.Data.Admin;

namespace backend.Tests.Shared
{
    public static class Helpers
    {
        public static AdminDbContext GetInMemoryAdminDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AdminDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new AdminDbContext(options);
            context.Database.EnsureDeleted(); // clean slate for each test
            context.Database.EnsureCreated();

            return context;
        }
    }
}
