using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Localization;

namespace OrchardCore.Localization.DataAnnotations
{
    public class LocalizedValidationMetadataProvider : IValidationMetadataProvider
    {
        private readonly IStringLocalizer _stringLocalizer;

        public LocalizedValidationMetadataProvider(IStringLocalizer stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }

        /// <remarks>This will localize the default data annotations error message if it is exist, otherwise will try to look for a parameterized version.</remarks>
        /// <example>
        /// A property named 'UserName' that decorated with <see cref="RequiredAttribute"/> will be localized using
        /// "The {0} field is required." and "The UserName field is required." error messages.
        /// </example>
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            foreach (var metadata in context.ValidationMetadata.ValidatorMetadata)
            {
                var attribute = metadata as ValidationAttribute;
                var argument = context.Key.Name;
                var errorMessageString = attribute != null && attribute.ErrorMessage == null && attribute.ErrorMessageResourceName == null
                    ? attribute.FormatErrorMessage(argument)
                    : attribute.ErrorMessage;

                // Localize the error message without params
                var localizedString = _stringLocalizer[errorMessageString];

                if (localizedString == errorMessageString)
                {
                    // Localize the error message with params
                    errorMessageString = errorMessageString.Replace(argument, "{0}");
                    localizedString = _stringLocalizer[errorMessageString, argument];
                }

                attribute.ErrorMessage = localizedString;
            }
        }
    }
}
