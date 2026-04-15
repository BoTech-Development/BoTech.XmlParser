namespace BoTech.XmlParser.Services;

public class TypeConverter
{
    /// <summary>
    /// Converts an object to the specified target type. If the target type is an enumeration
    /// and the provided object is a string, it converts the string to the corresponding enumeration value.
    /// Otherwise, it attempts to change the type using standard conversion methods.
    /// </summary>
    /// <param name="obj">
    /// The object to be converted. This may be null or of any type.
    /// </param>
    /// <param name="type">
    /// The target type to which the object should be converted. This can be a standard type or an enumeration.
    /// </param>
    /// <returns>
    /// The object converted to the specified target type, or null if the input object is null. If the conversion fails,
    /// an exception may be thrown.
    /// </returns>
    public static object? ConvertObjectToSpecificType(object? obj, Type type)
    {
        string? str = obj as string;
        if (type.IsEnum && str != null)
        {
            return Enum.Parse(type, str);
        }
        return Convert.ChangeType(obj, type);
    }
}