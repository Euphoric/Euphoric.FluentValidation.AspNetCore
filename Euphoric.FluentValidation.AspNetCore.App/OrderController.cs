using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
// ReSharper disable UnusedType.Global

namespace Euphoric.FluentValidation.AspNetCore;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> _logger;

    public OrderController(ILogger<OrderController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ModelValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public string Post([FromBody]Order order)
    {
        return "OK";
    }

    [HttpGet("error")]
    [ProducesResponseType(typeof(ModelValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    public string GetError()
    {
        var validationFailure = new ValidationFailure("CustomProperty", "Custom validation error.", "30");
        validationFailure.ErrorCode = "CUSTOM_VALIDATION";
        throw new ValidationException(new List<ValidationFailure> { validationFailure });
    }
}

public record Order(string? Description, string CustomerName, int Number, OrderItem[]? Items, OrderDetail OrderDetail);

public record OrderItem(string? Name, int Amount, decimal Price);

public record OrderDetail(string Address, string OrderName);

public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator(IValidator<OrderItem> orderItemValidator, IValidator<OrderDetail> orderDetailValidator)
    {
        RuleFor(o => o.CustomerName).NotEmpty();
        RuleFor(o => o.Description).NotEmpty().Length(1, 100);
        RuleFor(o => o.Number).ExclusiveBetween(10, 20);
        RuleFor(o => o.CustomerName).MustAsync((o, _) => Task.FromResult(o == "valid-name"))
            .WithMessage("Asynchronous error - '{PropertyValue}' is not valid value for {PropertyName}")
            .WithErrorCode("MyErrorCode");
        RuleFor(o => o.Items).NotEmpty();
        RuleFor(o => o.OrderDetail).NotEmpty().SetValidator(orderDetailValidator);
        RuleForEach(o => o.Items).SetValidator(orderItemValidator);
    }
}

public class OrderDetailValidator : AbstractValidator<OrderDetail>
{
    public OrderDetailValidator()
    {
        RuleFor(o => o.Address).NotEmpty();
        RuleFor(o => o.OrderName).NotEmpty();
    }
}

public class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator()
    {
        RuleFor(o => o.Name).NotEmpty();
        RuleFor(o => o.Price).ExclusiveBetween(0m, 10000m);
    }
}