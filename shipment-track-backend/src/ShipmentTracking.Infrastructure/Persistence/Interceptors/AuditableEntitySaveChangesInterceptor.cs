using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShipmentTracking.Application.Common.Interfaces;
using ShipmentTracking.Domain.Common;

namespace ShipmentTracking.Infrastructure.Persistence.Interceptors;

/// <summary>
/// EF Core interceptor that automatically populates audit fields.
/// </summary>
public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuditableEntitySaveChangesInterceptor(
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = _dateTimeProvider.UtcNow;
        var userId = _currentUserService.UserId;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.Entity is not IAuditableEntity auditableEntity)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditableEntity.SetCreatedAudit(userId, now);
                    auditableEntity.SetUpdatedAudit(userId, now);
                    break;
                case EntityState.Modified:
                    auditableEntity.SetUpdatedAudit(userId, now);
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.SoftDelete();
                    auditableEntity.SetUpdatedAudit(userId, now);
                    break;
                default:
                    if (entry.HasChangedOwnedEntities())
                    {
                        auditableEntity.SetUpdatedAudit(userId, now);
                    }
                    break;
            }
        }
    }
}

internal static class EntityEntryExtensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(reference =>
            reference.TargetEntry is { } target &&
            target.Metadata.IsOwned() &&
            target.State is EntityState.Added or EntityState.Modified);
    }
}
