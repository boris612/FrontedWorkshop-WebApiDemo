﻿using Backend.Core.Commands;
using Backend.WebApi.Util.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Backend.WebApi.Util.ExceptionFilters
{
  public class BadRequestOnRuleValidationException : ExceptionFilterAttribute
  {
    private readonly ILogger<BadRequestOnRuleValidationException> logger;

    public BadRequestOnRuleValidationException(ILogger<BadRequestOnRuleValidationException> logger)
    {
      this.logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
      if (context.Exception is ValidationException)
      {
        
        string exceptionMessage = context.Exception.CompleteExceptionMessage();        
        logger.LogDebug("Validation error: {0}", exceptionMessage);
        
        ValidationException exc = (ValidationException)context.Exception;
        Dictionary<string, List<string>> validationErrors = new Dictionary<string, List<string>>();

        foreach(var failure in exc.Errors)
        {
          //remove prefix Dto (part of Update and AddCommand)
          validationErrors.GetOrCreate(failure.PropertyName.Replace(nameof(AddCommand<object>.Dto), "")).Add(failure.ErrorMessage);
        }

        var problemDetails = new ValidationProblemDetails(validationErrors.ToDictionary(d => d.Key, d => d.Value.ToArray()))
        {
          Detail = context.Exception.Message,
          Title = "Validation exception",
          Instance = context.HttpContext.TraceIdentifier
        };
        context.Result = new ObjectResult(problemDetails)
        {
          ContentTypes = { "application/problem+json" },
          StatusCode = StatusCodes.Status400BadRequest
        };

        context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.ExceptionHandled = true;

      }     
    }   
  }
}
