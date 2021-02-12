﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using Backend.WebApi.Util.Extensions;

namespace Backend.WebApi.Util.ExceptionFilters
{
  public class ProblemDetailsForSqlException : ExceptionFilterAttribute
  {
    private readonly ILogger<ProblemDetailsForSqlException> logger;

    public ProblemDetailsForSqlException(ILogger<ProblemDetailsForSqlException> logger)
    {
      this.logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
      if (context.Exception is SqlException || context.Exception?.InnerException is SqlException)
      {
        string exceptionMessage = context.Exception.CompleteExceptionMessage();
        logger.LogDebug("SQL Exception {0}", exceptionMessage);
        context.ExceptionHandled = true;
        var problemDetails = new ProblemDetails
        {
          Detail = exceptionMessage,
          Title = "SqlException",
          Instance = context.HttpContext.TraceIdentifier
        };
        context.Result = new ObjectResult(problemDetails)
        {
          ContentTypes = { "application/problem+json" },
          StatusCode = StatusCodes.Status500InternalServerError
        };
      }      
    }
  }
}
