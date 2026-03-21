namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout;

public interface ILayoutElement : IFormElement
{
    public List<IFormElement> Children { get; }
}