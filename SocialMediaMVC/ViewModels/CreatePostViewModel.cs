using System.ComponentModel.DataAnnotations;

namespace SocialMediaMVC.ViewModels
{
    public class CreatePostViewModel
    {
        [ContentValidation]
        public string? Content { get; set; } = string.Empty;
        // TODO Validate if the images are of type image
        public List<IFormFile>? Images { get; set; } = new List<IFormFile>();
    }

    public class ContentValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = (CreatePostViewModel)validationContext.ObjectInstance;
            
            if (string.IsNullOrWhiteSpace(model.Content) && (model.Images is null || model.Images.Count == 0))
            {
                return new ValidationResult("Content or images are required");
            }

            return ValidationResult.Success;
        }
    }
}
