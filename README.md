# Alternative Fluent Validations for Asp.Net Core
An integration of FluentValidations into ASP.NET Core not using standard validation model.



## Swagger/Swashbuckle - Marking endpoints as returning error response types

Add `[ProducesResponseType(typeof(ModelValidationProblemDetails), (int)HttpStatusCode.BadRequest)]` to endpoints which will produce error responses. Either by having a validator for it's request DTO or by throwing an `ValidationException`