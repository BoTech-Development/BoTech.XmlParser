namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout.Grid;

public class ARow : IFormElement
{
    public List<AColumn> Columns { get; set; } = new List<AColumn>();
    public void Show()
    {
        throw new NotImplementedException();
    }
}