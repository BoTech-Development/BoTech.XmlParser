using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.DateTime;
[XmlName("DateInput")]
public class ADateInput : IInput<DateOnly>
{
    public string Description { get; init; }
    public string Name { get; init; }
    public string Property { get; init; }
    public DateOnly Value { get; set; }

    public void Show()
    {
        throw new NotImplementedException();
    }
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}