using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input;
[XmlName("Input")]
public class AInput : IInput<object>
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Property { get; set; }
    public object Value { get; set; }
    public void Show()
    {
        throw new NotImplementedException();
    }

    public void EvaluateType(){}
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}