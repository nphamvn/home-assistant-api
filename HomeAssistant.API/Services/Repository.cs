using System.Linq.Expressions;
using HomeAssistant.API.Data;
using HomeAssistant.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HomeAssistant.API.Services;
public class Repository<TModel> : IRepository<TModel>
    where TModel : class
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<TModel> _dbSet;

    public Repository(ApplicationDbContext dbContext, DbSet<TModel> dbSet)
    {
        _dbContext = dbContext;
        _dbSet = dbSet;
    }

    public async Task<IList<TModel>> GetAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IList<TModel>> Find(Expression<Func<TModel, Boolean>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IList<TModel>> Find<TProperty>(Expression<Func<TModel, Boolean>> predicate, Expression<Func<TModel, TProperty>> include)
    {
        return await _dbSet.Where(predicate).Include(include).ToListAsync();
    }

    public async Task<TModel> Single(Expression<Func<TModel, Boolean>> predicate)
    {
        return await _dbSet.SingleOrDefaultAsync(predicate);
    }

    public async Task<TModel> Single<TProperty>(Expression<Func<TModel, Boolean>> predicate, Expression<Func<TModel, TProperty>> include)
    {
        return await _dbSet.Where(predicate).Include(include).FirstOrDefaultAsync();
    }

    public virtual async Task<TModel> Create(TModel toCreate)
    {
        //toCreate.Id = Guid.NewGuid();
        await _dbSet.AddAsync(toCreate);
        await _dbContext.SaveChangesAsync();
        return toCreate;
    }

    public async Task Update(TModel toUpdate)
    {
        InternalUpdate(toUpdate);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(IList<TModel> toUpdate)
    {
        foreach (var entity in toUpdate)
        {
            InternalUpdate(entity);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(TModel toRemove)
    {
        InternalRemove(toRemove);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Remove(IList<TModel> toRemove)
    {
        foreach (var entity in toRemove)
        {
            InternalRemove(entity);
        }
        await _dbContext.SaveChangesAsync();
    }

    private void InternalUpdate(TModel toUpdate)
    {
        // var trackedEntity = _dbContext.ChangeTracker.Entries<TModel>().SingleOrDefault(x => x.Entity.Id == toUpdate.Id);
        // if (trackedEntity != null)
        // {
        //     trackedEntity.State = EntityState.Detached;
        // }

        // var entry = _dbContext.Entry(toUpdate);
        // entry.State = EntityState.Modified;
        // entry.Property("SysId").IsModified = false;

        _dbSet.Update(toUpdate);
        _dbContext.SaveChanges();
    }

    private void InternalRemove(TModel toRemove)
    {
        // var trackedEntity = _dbContext.ChangeTracker.Entries<TModel>().SingleOrDefault(x => x.Entity.Id == toRemove.Id);
        // if (trackedEntity != null)
        // {
        //     trackedEntity.State = EntityState.Detached;
        // }
        // _dbContext.Entry(toRemove).State = EntityState.Deleted;

        _dbSet.Remove(toRemove);
        _dbContext.SaveChanges();
    }
}