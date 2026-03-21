namespace BoTech.XmlParser.Tests.TestModels.Controls;

public interface IContentElement : IFormElement
{
    public IFormElement Content { get; set; }
}