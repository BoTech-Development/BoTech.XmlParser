using System.Numerics;
using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Input.Numeric;
[XmlName("NumberInput")]
public class ANumberInput<T> : IInput<T> where T : INumber<T>
{
    public string Description { get; set; }
    public string Name { get; set; }
    public string Property { get; set; }
    public T Value { get; set; }
    
    public void Show()
    {
        throw new NotImplementedException();
    }
    public void OnUserUpdatedValue()
    {
        throw new NotImplementedException();
    }
}