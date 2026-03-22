using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout.Grid;
[XmlName("Column")]
public class AColumn : IContentElement
{
    public IFormElement Content { get; set; }
    public void Show()
    {
        throw new NotImplementedException();
    }
}