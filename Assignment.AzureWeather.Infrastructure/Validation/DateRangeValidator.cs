using Assignment.AzureWeather.Infrastructure.DTO.Requests;
using FluentValidation;

namespace Assignment.AzureWeather.Infrastructure.Validation;

public class DateRangeValidator : AbstractValidator<GetWeatherRequest>
{
    public DateRangeValidator()
    {
        RuleFor(x => x).Custom((model, context) =>
        {
            if (model.From.HasValue && model.To.HasValue && model.From.Value > model.To.Value)
            {
                context.AddFailure("End date must be greater than start date");
            }

            if (model.From.HasValue && (model.From == DateTime.MaxValue || model.From == DateTime.MinValue))
            {
                context.AddFailure(nameof(model.From), "Invalid start date");
            }
            
            if (model.To.HasValue && (model.To == DateTime.MaxValue || model.To == DateTime.MinValue))
            {
                context.AddFailure(nameof(model.To), "Invalid end date");
            }
        });
    }
}