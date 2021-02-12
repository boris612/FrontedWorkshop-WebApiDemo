using Backend.Contract.DTOs;
using Backend.Core.Commands;
using Backend.Core.Queries.Generic;
using Backend.Core.Validation;
using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.CommandValidators
{
  public class AddWorkshopParticipant : AbstractValidator<AddCommand<WorkshopParticipant>>
  {
    private readonly IMediator mediator;

    public AddWorkshopParticipant(IMediator mediator)
    {
      this.mediator = mediator;

      var uniqueIndexValidator = new UniqueIndexValidator<WorkshopParticipant>(mediator,
        t => t.StudentId.ToString(),
        t => t.WorkshopId.ToString());
      
      RuleFor(a => a.Dto.StudentId)                
        .ForeignKeyExists<AddCommand<WorkshopParticipant>, Student>(mediator) //check that Student exists
        .DependentRules(() => 
            RuleFor(a => a.Dto.WorkshopId)
              .Cascade(CascadeMode.Stop)
              .ForeignKeyExists<AddCommand<WorkshopParticipant>, Workshop> (mediator)  //check that Workshop exists
              .MustAsync(HaveFreePlaces).WithMessage("Workshop does not have free places") //check free places
              .DependentRules(() => 
                    RuleFor(a => a.Dto).CustomAsync(uniqueIndexValidator.Validate) //check unique index
               )              
        );      
    }

    private async Task<bool> HaveFreePlaces(int workshopId, CancellationToken cancellationToken)
    {
      var query = new GetSingleItemQuery<Workshop>(workshopId);
      var item = await mediator.Send(query, cancellationToken);
      return item != null && item.FreePlaces > 0;
    }
  }
}
