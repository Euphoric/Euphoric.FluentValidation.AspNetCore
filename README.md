# Alternative Fluent Validations integration for Asp.Net Core
An integration of FluentValidations into Asp.Net Core not using standard validation model.

## The problem

Native integration between Fluent Validations and Asp.Net Core's model validation is extremely flawed. Even [Fluent Validation recommends not using it](https://docs.fluentvalidation.net/en/latest/aspnet.html). 

Most troubling flaws are:
* No async support. If a validator contains async validation, it will cause errors.
* Returned response structures are biased toward how Asp.Net core represents validation errors internally and not how good error details API should be. Thus Fluent Validation cannot return all relevant validation details, like error code.

## The solution

This library integrates with Asp.Net MVC's pipeline and runs Fluent Validation outside standard model validation. This allows it to run asynchronously and return custom validation response structures, which contain all relevant validation data.

**WARNING** To avoid conflicts with standard validation pipeline, it is turned off. This might result in some validations stop working. These need to be replaced with Fluent or manual validations.

## How to

### Usage

To start using this library, import it from a **NuGet** `Euphoric.FluentValidation.AspNetCore`

Then add it to services `service.AddAutoFluentValidations();`

Then, as long as a MVC endpoint's request DTO has a corresponding `FluentValidation.IValidator<TRequest>`, the validator will be run for each request.

When an `FluentValidation.ValidationException` is thrown from a controller, it will also be handled and returned as a validation error.

### Response structure

The returned error response structure example is based on standard `ProblemDetail` response.

Example:

    {
      "errors": [
        {
          "propertyName": "Description",
          "errorCode": "LengthValidator",
          "attemptedValue": "01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789",
          "errorMessage": "'Description' must be between 1 and 100 characters. You entered 110 characters.",
          "MinLength": 1,
          "MaxLength": 100,
          "TotalLength": 110
        },
        {
          "propertyName": "Number",
          "errorCode": "ExclusiveBetweenValidator",
          "attemptedValue": 25,
          "errorMessage": "'Number' must be between 10 and 20 (exclusive). You entered 25.",
          "From": 10,
          "To": 20
        },
        {
          "propertyName": "CustomerName",
          "errorCode": "MyErrorCode",
          "attemptedValue": "invalid-name",
          "errorMessage": "Asynchronous error - 'invalid-name' is not valid value for Customer Name"
        },
        {
          "propertyName": "OrderDetail.Address",
          "errorCode": "NotEmptyValidator",
          "attemptedValue": "",
          "errorMessage": "'Address' must not be empty."
        },
        {
          "propertyName": "OrderDetail.OrderName",
          "errorCode": "NotEmptyValidator",
          "attemptedValue": null,
          "errorMessage": "'Order Name' must not be empty."
        },
        {
          "propertyName": "Items[0].Name",
          "errorCode": "NotEmptyValidator",
          "attemptedValue": "",
          "errorMessage": "'Name' must not be empty."
        },
        {
          "propertyName": "Items[0].Price",
          "errorCode": "ExclusiveBetweenValidator",
          "attemptedValue": 1000000,
          "errorMessage": "'Price' must be between 0 and 10000 (exclusive). You entered 1000000.",
          "From": 0,
          "To": 10000
        }
      ],
      "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
      "title": "One or more validation errors occurred.",
      "status": 400,
      "detail": "'Description' must be between 1 and 100 characters. You entered 110 characters. 'Number' must be between 10 and 20 (exclusive). You entered 25. Asynchronous error - 'invalid-name' is not valid value for Customer Name 'Address' must not be empty. 'Order Name' must not be empty. 'Name' must not be empty. 'Price' must be between 0 and 10000 (exclusive). You entered 1000000.",
      "instance": null,
      "traceId": "00-37be1758609afda059cc901e1ba893ec-509bca209ccef79e-00"
    }

### Swagger/Swashbuckle - Marking endpoints as returning error response types

Add `[ProducesResponseType(typeof(ModelValidationProblemDetails), (int)HttpStatusCode.BadRequest)]` to endpoints which will produce error responses. Either by having a validator for it's request DTO or by throwing an `ValidationException`