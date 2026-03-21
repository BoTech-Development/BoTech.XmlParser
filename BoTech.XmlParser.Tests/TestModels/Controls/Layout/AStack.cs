namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout;

public class AStack : ILayoutElement
{

    public required List<IFormElement> Children { get; init; } = new List<IFormElement>();
    public Orientation Orientation { get; init; } = Orientation.Vertical;
    public void Show()
    {
        throw new NotImplementedException();
    }

    
}