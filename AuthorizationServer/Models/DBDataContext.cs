using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Models
{
    public class DBDataContext : IdentityDbContext
    {
        public DBDataContext(DbContextOptions<DBDataContext> options) : base(options) { }
    }
}