using System.Linq.Expressions;

namespace CinemaHikes.Domain.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    List<Expression<Func<T, object>>> ThenBys { get; }
    int Skip { get; }
    int Take { get; }
    bool IsPagingEnabled { get; }
}