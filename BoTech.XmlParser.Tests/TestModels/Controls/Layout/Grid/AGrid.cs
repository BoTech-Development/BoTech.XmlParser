namespace BoTech.XmlParser.Tests.TestModels.Controls.Layout.Grid;
public class AGrid : IFormElement
{
    public List<ARow> Rows { get; set; } = new List<ARow>();
    public void Show()
    {
        throw new NotImplementedException();
    }
}