using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Backend.DAL_EF.Core
{
  public static class GenericFilterExtensions
  {
    private static readonly MethodInfo Contains = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
    private static readonly MethodInfo StartsWith = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });    
    private static readonly MethodInfo EndsWith = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
  
    /// <summary>
    /// Create where predicated for the column
    /// Important note! Does not work with nullable numeric columns (e.g. int null, ...)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="path">column name, or Table.ColumnName (in case of foreign key attribute)</param>
    /// <param name="value"></param>
    /// <param name="operator"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> BuildWherePredicate<T>(this Expression<Func<T, object>> columnSelector, string value, string @operator, ILogger logger = null                                                                          )
    {      
      Expression body = null;
      MemberExpression left = null;
      if (columnSelector.Body is UnaryExpression)
      {
        left = ((UnaryExpression)columnSelector.Body).Operand as MemberExpression; //zbog value tipova
      }
      else
      {
        left = columnSelector.Body as MemberExpression;
      }

      PropertyInfo propertyInfo = left.Member as PropertyInfo;
      ConstantExpression constant = null;
      if (propertyInfo.PropertyType.IsValueType)
      {
        try { 
          object converted = Convert.ChangeType(value, propertyInfo.PropertyType);
          constant = Expression.Constant(converted);
        }
        catch(Exception exc)
        {
          logger.LogWarning(exc, $"Cannot convert value {value} for field {columnSelector} to {propertyInfo.PropertyType.Name} in {typeof(T).Name}");
        }            
      }
      else
      {
        constant = Expression.Constant(value);
      }

      if (constant == null) return null;
            
      switch (@operator)
      {
        case "=":
        case "equals":
          body = Expression.Equal(left, constant);
          break;
        case "<":
          body = Expression.LessThan(left, constant);
          break;
        case "<=":
          body = Expression.LessThanOrEqual(left, constant);
          break;
        case ">=":
          body = Expression.GreaterThanOrEqual(left, constant);
          break;
        case ">":
          body = Expression.GreaterThan(left, constant);
          break;
        case "<>":
          body = Expression.NotEqual(left, constant);
          break;
        case "contains":
          body = Expression.Call(left, Contains, constant);
          break;
        case "startswith":
          body = Expression.Call(left, StartsWith, constant);
          break;
        case "endswith":
          body = Expression.Call(left, EndsWith, constant);
          break;
        default:
          logger.LogWarning($"Invalid operator ({@operator}) for path {columnSelector} and value={value} for type {typeof(T).Name}");
          break;
      }

     // if (negate) body = Expression.Not(ret);
     
      Expression<Func<T, bool>> predicate = null;
      if (body != null)
      {
        predicate = Expression.Lambda<Func<T, bool>>(body, columnSelector.Parameters.First());
      }
      return predicate;
    }    
  }
}
