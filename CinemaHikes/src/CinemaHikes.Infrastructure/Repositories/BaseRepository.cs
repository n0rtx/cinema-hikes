using CinemaHikes.Domain.Interfaces.Repositories;
using CinemaHikes.Domain.Specifications;
using CinemaHikes.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CinemaHikes.Infrastructure.Repositories;

public class BaseRepository<T>(CinemaHikesDbContext dbContext) : IRepository<T> where T : class
{
    protected readonly CinemaHikesDbContext DbContext = dbContext;

    protected readonly DbSet<T> DbSet = dbContext.Set<T>();

    public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => await DbSet.FindAsync([id], cancellationToken);

    public async Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        => await ApplySpecification(spec, true).ToListAsync(cancellationToken);

    public async Task<int> CountAsync(ISpecification<T> spec, CancellationToken cancellationToken)
        => await ApplySpecification(spec, true).CountAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
        => await DbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity) => DbSet.Update(entity);

    public void Remove(T entity) => DbSet.Remove(entity);

    private IQueryable<T> ApplySpecification(ISpecification<T> spec, bool applyPaging)
    {
        var query = DbSet.AsQueryable();

        if (spec.Criteria is not null) query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
        {
            query = query.Include(include);
        }

        if (spec.OrderBy is not null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending is not null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (applyPaging && spec.IsPagingEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        return query;
    }
}