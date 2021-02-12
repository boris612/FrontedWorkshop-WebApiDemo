using Backend.Contract.DTOs;
using Backend.Core.Commands;
using Backend.Core.Validation;
using FluentValidation;
using MediatR;

namespace Backend.CommandValidators
{
  public class AddTownValidator : AbstractValidator<AddCommand<Town>>
  {
    public AddTownValidator(IMediator mediator)
    {
      var uniqueIndexValidator = new UniqueIndexValidator<Town>(mediator, t => t.Postcode.ToString(), t => t.Name);
      //RuleFor(a => a.Dto.Name).NotEmpty().DependentRules(
      //  () =>
      //  {
      //    RuleFor(a => a.Dto.Name).CustomAsync(new UniqueIndexValidator<Town>(mediator, t => t.Name).Validate);
      //  });  

      RuleFor(a => a.Dto.Name).NotEmpty().DependentRules(() =>
        RuleFor(a => a.Dto.Postcode).NotEmpty().DependentRules(() =>
          RuleFor(a => a.Dto).CustomAsync(uniqueIndexValidator.Validate))
        );            
    }
  }
}
