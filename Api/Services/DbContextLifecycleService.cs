using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Api.Models.BaseClasses;

namespace Api.Services
{
    public interface IDbContextLifecycleService
    {
        void UpdateTimestamps(ChangeTracker changeTracker);
    }

    public class DbContextLifecycleService : IDbContextLifecycleService
    {
        public void UpdateTimestamps(ChangeTracker changeTracker)
        {
            var entries = changeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedUtc = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedUtc = DateTime.UtcNow;
                }
            }
        }
    }
}