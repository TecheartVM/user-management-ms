using MongoDB.Driver;
using UserManagement.Domain.Exceptions;

namespace UserManagement.Infrastructure.DataAccess.MongoDb;

internal static class MongoExtensions
{
    public static string? GetDuplicatePropertyName(this MongoWriteException ex)
    {
        if (ex == null) return null;

        if (ex.Message.Equals(string.Empty))
            return null;

        var result = ex.Message.Split("dup key:")[1];
        result = result.Split(':')[0].Split('{')[1].Trim();

        return result.Equals(string.Empty) ? null : result;
    }

    public static Exception WrapException(this MongoWriteException ex)
    {
        WriteError error = ex.WriteError;
        if (error?.Category == ServerErrorCategory.DuplicateKey)
        {
            return new DuplicateModelDataException(
                ex.GetDuplicatePropertyName());
        }
        return new Exception(ex.Message);
    }
}
