using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout;
[XmlName("Stack")]
public class AStack : ILayoutElement
{

    public required List<IFormElement> Children { get; init; } = new List<IFormElement>();
    public Orientation Orientation { get; init; } = Orientation.Vertical;
    public void Show()
    {
        throw new NotImplementedException();
    }

    
}