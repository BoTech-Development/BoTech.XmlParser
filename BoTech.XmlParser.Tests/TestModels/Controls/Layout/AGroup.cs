using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout;
[XmlName("Group")]
public class AGroup : IContentElement, ITitle
{
    public string Title { get; init; }
    public string SubTitle { get; init; }
    public IFormElement Content { get; set; }

    public void Show()
    {
        throw new NotImplementedException();
    }
}