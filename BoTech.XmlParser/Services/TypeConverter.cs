namespace BoTech.XmlParser.Services;

public class TypeConverter
{
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