namespace UserManagement.Domain.Exceptions;

public class ConfigurationSectionNotFoundException : Exception
{
	public ConfigurationSectionNotFoundException(string sectionName)
		: base($"The '{sectionName}' section not found in configuration sources.")
	{
	}
}
