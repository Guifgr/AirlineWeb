using Microsoft.EntityFrameworkCore;
using TravelAgentWeb.Model;

namespace TravelAgentWeb.Data
{
    public class TravelAgentDbContext : DbContext
    {
        public TravelAgentDbContext(DbContextOptions<TravelAgentDbContext> opt) : base(opt)
        {
            
        }
        
        public DbSet<WebhookSecret> WebhookSecrets { get; set; }
    }
}