using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.DateTime;
[XmlName("TimeInput")]
public class ATimeInput : IInput<TimeOnly>
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Property { get; set; }
    public TimeOnly Value { get; set; }
    public void Show()
    {
        throw new NotImplementedException();
    }
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}