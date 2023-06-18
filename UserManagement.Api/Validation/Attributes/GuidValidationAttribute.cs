using System.ComponentModel.DataAnnotations;

namespace UserManagement.Api.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    internal class GuidIdAttribute : ValidationAttribute
    {
        public GuidIdAttribute() : base("The value is not a valid Guid id") { }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return false;

            if (value is Guid guid)
                return guid != Guid.Empty;

            return false;
        }
    }
}
