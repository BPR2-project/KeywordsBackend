using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Keywords.Extensions;

public static class ModelStateExtensions
{
    public static List<string> GetAllErrors(this ModelStateDictionary controllerModelState)
    {
        var modelErrors = new List<string>();
        foreach (var modelState in controllerModelState.Values)
        foreach (var modelError in modelState.Errors)
            modelErrors.Add(modelError.ErrorMessage);

        return modelErrors;
    }
}