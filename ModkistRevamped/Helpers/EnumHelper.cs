using System.ComponentModel;
using System.Globalization;
using TNRD.Modkist.Models;

namespace TNRD.Modkist.Helpers;

public static class EnumHelper
{
    public static string Description(this Enum value)
    {
        object[] attributes = value.GetType()
            .GetField(value.ToString())!
            .GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Any())
            return (attributes.First() as DescriptionAttribute)!.Description;

        // If no description is found, the least we can do is replace underscores with spaces
        // You can add your own custom default formatting logic here
        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
        return ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));
    }

    public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException($"{nameof(type)} must be an enum type");

        return Enum.GetValues(type)
            .Cast<Enum>()
            .Select(e => new ValueDescription()
            {
                Value = e,
                Description = e.Description()
            })
            .ToList();
    }
}
