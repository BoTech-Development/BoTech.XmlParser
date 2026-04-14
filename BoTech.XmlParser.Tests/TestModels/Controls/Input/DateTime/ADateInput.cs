using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.DateTime;
[XmlName("DateInput")]
public class ADateInput : IInput<DateOnly>
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Property { get; set; }
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