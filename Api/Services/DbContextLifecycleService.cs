using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Api.Models.BaseClasses;

namespace Api.Services
{
    #region Interface
    /// <summary>
    /// Service for handling entity lifecycle events, such as updating timestamps.
    /// </summary>
    public interface IDbContextLifecycleService
    {
        /// <summary>
        /// Updates CreatedUtc and UpdatedUtc timestamps for tracked entities.
        /// Should be called before saving changes to the DbContext.
        /// </summary>
        /// <param name="changeTracker">The ChangeTracker from the DbContext.</param>
        void UpdateTimestamps(ChangeTracker changeTracker);
    }
    #endregion
    #region Implementation
    /// <inheritdoc/>
    public class DbContextLifecycleService : IDbContextLifecycleService
    {
        /// <inheritdoc/>
        public void UpdateTimestamps(ChangeTracker changeTracker)
        {
            // Iterate through all tracked entities of type BaseEntity
            var entries = changeTracker.Entries<BaseEntity>();
            foreach (var entry in entries)
            {
                // Set UpdatedUtc for modified entities
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedUtc = DateTime.UtcNow;
                }
                // Set CreatedUtc and UpdatedUtc for newly added entities
                else if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedUtc = DateTime.UtcNow;
                    entry.Entity.UpdatedUtc = DateTime.UtcNow;
                }
            }
        }
    }
    #endregion
}