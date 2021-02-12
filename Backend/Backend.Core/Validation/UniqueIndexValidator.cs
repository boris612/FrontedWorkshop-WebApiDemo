using Backend.Core.Commands;
using Backend.Core.Queries.Common;
using Backend.Core.Queries.Generic;
using FluentValidation.Validators;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Core.Validation
{
  public class UniqueIndexValidator<TDto> where TDto : IHasIntegerId
  {
    private readonly IMediator mediator;
    private readonly Expression<Func<TDto, string>>[] selectors;

    public UniqueIndexValidator(IMediator mediator, params Expression<Func<TDto, string>>[] selectors)
    {
      this.mediator = mediator;
      this.selectors = selectors;
    } 

    public async Task Validate(string value, CustomContext context, CancellationToken cancellationToken)
    {
      string columnName = GetColumnName(context);

      var query = new GetCountQuery<TDto>()
      {
        Filters = new List<Filter>()
      };
      query.Filters.Add(new Filter(columnName, "=", value.Trim()));

      int count = await mediator.Send(query, cancellationToken);
      if (count > 0)
      {
        context.AddFailure($"{columnName} must be unique. Value {value} has been already used!");
      }
    }   

    public async Task Validate(TDto value, CustomContext context, CancellationToken cancellationToken)
    {
      var query = new GetCountQuery<TDto>()
      {
        Filters = new List<Filter>()
      };

      List<string> columnNames = new List<string>();
      foreach (var selector in selectors)
      {
        string columnName = GetColumnName(selector);        
        columnNames.Add(columnName);
        query.Filters.Add(new Filter(columnName, "=", selector.Compile().Invoke(value)));
      }

      int count = await mediator.Send(query, cancellationToken);
      if (count > 0)
      {
        context.AddFailure($"n-tuple ({string.Join(", ", columnNames)}) must be unique.");
      }
    }

    public async Task ValidateExisting(string value, CustomContext context, CancellationToken cancellationToken)
    {
      string columnName = GetColumnName(context);

      UpdateCommand<TDto> validatingObject = (UpdateCommand<TDto>)context.InstanceToValidate;
      var query = new GetItemsQuery<TDto>()
      {
        Filters = new List<Filter>
        {
          new Filter(columnName, "=", value.Trim())
        }
      };
      IEnumerable<TDto> items = await mediator.Send(query, cancellationToken);
      if (items.Count() > 0)
      {

        bool valueBelongsToValidatingItem = items.Any(item => item.Id == validatingObject.Dto.Id);
        if (!valueBelongsToValidatingItem)
        {
          context.AddFailure($"{columnName} must be unique. Value {value} has been already used!");
        }
      }
    }

    public async Task ValidateExisting(TDto value, CustomContext context, CancellationToken cancellationToken)
    {      
      var query = new GetItemsQuery<TDto>()
      {
        Filters = new List<Filter>()
      };

      List<string> columnNames = new List<string>();
      foreach (var selector in selectors)
      {
        string columnName = GetColumnName(selector);
        columnNames.Add(columnName);
        query.Filters.Add(new Filter(columnName, "=", selector.Compile().Invoke(value).ToString()));
      }

      IEnumerable<TDto> items = await mediator.Send(query, cancellationToken);
      if (items.Count() > 0)
      {

        bool valueBelongsToValidatingItem = items.Any(item => item.Id == value.Id);
        if (!valueBelongsToValidatingItem)
        {
          context.AddFailure($"n-tuple ({string.Join(", ", columnNames)}) must be unique.");
        }
      }            
    }

    private string GetColumnName(Expression<Func<TDto, string>> expression)
    {
      if (expression.Body.NodeType == ExpressionType.MemberAccess)
      {
        var me = expression.Body as MemberExpression;
        return me.Member.Name;
      }
      else if (expression.Body.NodeType == ExpressionType.Call)
      {
        MethodCallExpression mce = expression.Body as MethodCallExpression;
        var me = mce.Object as MemberExpression;
        return me.Member.Name;        
      }
      else throw new Exception($"Invalid nodetype ({expression.NodeType}) in expression");
    }

    private string GetColumnName(CustomContext context)
    {
      if (selectors.Length != 1)
      {
        throw new Exception($"Unique index contains several columns, and must not be called on a single property {context.PropertyName}");
      }

      var selector = selectors[0];
      var me = selector.Body as MemberExpression;
      string columnName = me.Member.Name;
      if (columnName != context.PropertyName.Replace(nameof(UpdateCommand<TDto>.Dto) + "." , ""))
      {
        throw new Exception($"Unique index is defined on {columnName} but called on {context.PropertyName}");
      }

      return columnName;
    }
  }
}
