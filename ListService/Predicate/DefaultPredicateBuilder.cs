namespace ListService.Predicate
{
    using System;
    using System.Linq.Expressions;
    using Filters;
    using Models;

    public class DefaultPredicateBuilder<T> where T : IDefaults
    {
        public static Expression<Func<T, bool>> GetFilteredPredicate(Filter filter = null)
        {
            var predicate = PredicateBuilder.True<T>();

            if (filter == null)
            {
                return predicate;
            }

            if (filter.CreateDateTime.HasValue)
            {
                predicate = predicate.And(p => p.CreateDateTime >= filter.CreateDateTime.Value);
            }

            if (filter.LastUpdateDateTime.HasValue)
            {
                predicate = predicate.And(p => p.LastUpdateDateTime >= filter.LastUpdateDateTime.Value);
            }

            if (filter.CreatedBy.HasValue)
            {
                predicate = predicate.And(p => p.CreatedBy == filter.CreatedBy.Value);
            }

            if (filter.LastUpdateBy.HasValue)
            {
                predicate = predicate.And(p => p.LastUpdateBy == filter.LastUpdateBy.Value);
            }

            return predicate;
        }
    }
}