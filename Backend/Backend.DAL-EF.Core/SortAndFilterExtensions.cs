using Backend.Core.Queries.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Backend.DAL_EF.Core
{
  public static class SortAndFilterExtensions
  {
    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, SortOrder sortOrder, Dictionary<string, Expression<Func<T, object>>> orderSelectors)
    {
      if (sortOrder?.ColumnsOrder != null)
      {
        bool first = true;
        foreach (var sort in sortOrder.ColumnsOrder)
        {
          if (orderSelectors.TryGetValue(sort.Key.ToUpper(), out var orderSelector))
          {
            if (first)
            {
              query = sort.Value == SortOrder.Order.ASCENDING ?
                                          query.OrderBy(orderSelector)
                                        : query.OrderByDescending(orderSelector);
              first = false;
            }
            else
            {
              IOrderedQueryable<T> oquery = (IOrderedQueryable<T>)query;
              query = sort.Value == SortOrder.Order.ASCENDING ?
                                          oquery.ThenBy(orderSelector)
                                        : oquery.ThenByDescending(orderSelector);
            }
          }
        }
      }
      return query;
    }

    public static IQueryable<T> ApplyFilterData<T>(this IQueryable<T> query,
                                                   List<Filter> filters,
                                                   Func<string, string, string, Expression<Func<T, bool>>> createWherePredicate)
    {
      if (filters != null)
      {
        foreach (var filter in filters)
        {
          var predicate = createWherePredicate(filter.Column, filter.Operator, filter.Value);
          if (predicate != null)
          {
            query = query.Where(predicate);
          }
        }
      }

      return query;
    }
  }
}
