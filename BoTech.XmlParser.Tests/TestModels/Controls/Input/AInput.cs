using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input;
[XmlName("Input")]
public class AInput : IInput<object>
{
    public string Description { get; init; }
    public string Name { get; init; }
    public string Property { get; init; }
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