using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.DateTime;
[XmlName("DateTimeInput")]
public class ADateTimeInput : IInput<System.DateTime>
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Property { get; set; }
    public System.DateTime Value { get; set; }
    public void Show()
    {
        throw new NotImplementedException();
    }
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}