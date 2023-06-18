namespace UserManagement.Domain.Exceptions;

public class DuplicateModelDataException : Exception
{
	public readonly string? duplicatePropertyName;

	public DuplicateModelDataException(string? duplicatePropertyName = null)
		:base((duplicatePropertyName != null)
			? $"The {duplicatePropertyName} value must be unique."
			: "Invalid non-unique model property value.")
	{
		this.duplicatePropertyName = duplicatePropertyName;
	}
}

