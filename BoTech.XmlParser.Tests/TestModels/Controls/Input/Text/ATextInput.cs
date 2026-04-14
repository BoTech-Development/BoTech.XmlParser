using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.Text;
[XmlName("TextInput")]
public class ATextInput : IInput<string>
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Property { get; set; }
    public string Value { get; private set; }
    public void Show()
    {
        throw new NotImplementedException();
    }
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}