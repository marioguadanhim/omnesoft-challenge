using ErrorOr;

namespace OmnesoftChallenge.BLL.Utils.Validators;

public static class RequestValidator
{
    public static List<Error> ValidateLogin(string? userName, string? password)
    {
        List<Error> errors = [];
        ValidateRequiredText(errors, "UserName", userName, 100);
        ValidateRequiredText(errors, "Password", password, 100);
        return errors;
    }

    public static List<Error> ValidateRefreshToken(string? refreshToken)
    {
        List<Error> errors = [];
        ValidateRequiredText(errors, "RefreshToken", refreshToken, 500);
        return errors;
    }

    public static List<Error> ValidateProductId(int id)
    {
        return id > 0
            ? []
            : [Error.Validation("Id", "Id must be greater than 0.")];
    }

    public static List<Error> ValidateProduct(string? name, decimal price, string? description)
    {
        List<Error> errors = [];

        ValidateRequiredText(errors, "Name", name, 200);

        if (price <= 0)
        {
            errors.Add(Error.Validation("Price", "Price must be greater than 0."));
        }

        if (price >= 1000000000)
        {
            errors.Add(Error.Validation("Price", "Price must be less than 1,000,000,000."));
        }

        if (description is not null && description.Length > 1000)
        {
            errors.Add(Error.Validation("Description", "Description maximum size is 1000."));
        }

        return errors;
    }

    #region Private Helper Methods

    private static void ValidateRequiredText(List<Error> errors, string field, string? value, int maximumLength)
    {
        if (value is null)
        {
            errors.Add(Error.Validation(field, $"{field} cannot be null."));
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add(Error.Validation(field, $"{field} cannot be empty."));
        }

        if (value is not null && value.Length > maximumLength)
        {
            errors.Add(Error.Validation(field, $"{field} maximum size is {maximumLength}."));
        }
    }

    #endregion
}
