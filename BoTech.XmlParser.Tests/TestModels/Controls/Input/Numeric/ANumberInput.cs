using System.Numerics;
using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.Numeric;
[XmlName("NumberInput")]
public class ANumberInput<T> : IInput<T> where T : INumber<T>
{
    public string Description { get; init; }
    public string Name { get; init; }
    public string Property { get; init; }
    public T Value { get; private set; }
    
    public void Show()
    {
        throw new NotImplementedException();
    }
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}