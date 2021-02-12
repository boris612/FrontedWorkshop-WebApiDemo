using Backend.Core.Queries.Common;
using Backend.Core.Queries.Generic;
using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.Core.Validation
{
  public static class ForeignKeyValueValidatorExtension 
  {   
    public static IRuleBuilderOptions<TCommand, int> ForeignKeyExists<TCommand, TDto>(this IRuleBuilder<TCommand, int> ruleBuilder, IMediator mediator) where TDto : IHasIntegerId
    {
      return ruleBuilder.MustAsync(new ForeignKeyValueValidator<TDto>(mediator).Validate)
                        .WithMessage("Main entity {PropertyName} = {PropertyValue} does not exist!");
    }

    private class ForeignKeyValueValidator<TDto> where TDto : IHasIntegerId
    {
      private readonly IMediator mediator;

      public ForeignKeyValueValidator(IMediator mediator)
      {
        this.mediator = mediator;
      }

      public async Task<bool> Validate(int value, CancellationToken cancellationToken)
      {
        var query = new GetCountQuery<TDto>()
        {
          Filters = new List<Filter>
          {
            new Filter(nameof(IHasIntegerId.Id), "=", value.ToString())
          }          
        };
        int count = await mediator.Send(query, cancellationToken);
        return count > 0;
      }

      //an alternative using CustomAsync
      //public async Task Validate(int value, CustomContext context, CancellationToken cancellationToken)
      //{
      //  var query = new GetCountQuery<TDto>()
      //  {
      //    Filter = new Dictionary<string, string>
      //    {
      //      [nameof(IHasIntegerId.Id)] = value.ToString()
      //    }
      //  };
      //  int count = await mediator.Send(query, cancellationToken);
      //  if (count == 0)
      //  {
      //    context.AddFailure($"Main entity {context.PropertyName} = {value} does not exist!");
      //  }
      //}
    }
  }
}
