using Backend.Contract.DTOs;
using Backend.Core.Commands;
using Backend.Core.Validation;
using FluentValidation;
using MediatR;

namespace PlugAndPlay.CommandValidators
{
  public class UpdateTownValidator : AbstractValidator<UpdateCommand<Town>>
  {
    public UpdateTownValidator(IMediator mediator)
    {
      var uniqueIndexValidator = new UniqueIndexValidator<Town>(mediator, t => t.Postcode.ToString(), t => t.Name);
       RuleFor(a => a.Dto.Name).NotEmpty().DependentRules(() =>
        RuleFor(a => a.Dto.Postcode).NotEmpty().DependentRules(() =>
          RuleFor(a => a.Dto).CustomAsync(uniqueIndexValidator.ValidateExisting))
        );     
    }     
  }
}
