using BoTech.XmlParser.Tests.TestModels.Models.Import;

namespace BoTech.XmlParser.Tests.TestModels.Controls;

public class BoForm
{
    public Form FormContent { get; init; } = new();
    public List<ImportBase> Imports { get; init; } = new List<ImportBase>();
}