using System.Linq.Expressions;

namespace HomeAssistant.API.Services.Interfaces;
public interface IRepository<TModel>
    where TModel : class
{
    Task<IList<TModel>> GetAll();

    Task<IList<TModel>> Find(Expression<Func<TModel, Boolean>> predicate);

    Task<IList<TModel>> Find<TProperty>(Expression<Func<TModel, Boolean>> predicate, Expression<Func<TModel, TProperty>> include);

    Task<TModel> Single(Expression<Func<TModel, Boolean>> predicate);
    Task<TModel> Single<TProperty>(Expression<Func<TModel, Boolean>> predicate, Expression<Func<TModel, TProperty>> include);

    Task<TModel> Create(TModel toCreate);

    Task Update(TModel toUpdate);

    Task Update(IList<TModel> toUpdate);

    Task Remove(TModel toRemove);

    Task Remove(IList<TModel> toRemove);
}