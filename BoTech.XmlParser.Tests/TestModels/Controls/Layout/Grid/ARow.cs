using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout.Grid;
[XmlName("Row")]
public class ARow : IFormElement
{
    public List<AColumn> Columns { get; set; } = new List<AColumn>();
    public void Show()
    {
        throw new NotImplementedException();
    }
}