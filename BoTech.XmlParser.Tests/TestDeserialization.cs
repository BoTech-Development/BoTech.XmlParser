using BoTech.XmlParser.Helper.Serializer;
using BoTech.XmlParser.Models.Deserializer;
using BoTech.XmlParser.Services;
using BoTech.XmlParser.Tests.TestModels.Controls;
using BoTech.XmlParser.Tests.TestModels.Models.Import;
using NUnit.Framework;

namespace BoTech.XmlParser.Tests;

public class TestDeserialization
{
    private string _sampleFormAsXml;
    [SetUp]
    public void Setup()
    {
        _sampleFormAsXml =
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<BoForm>\n    <BoForm.FormContent>\n        <Form IsStepped=\"True\">\n            <Step Title=\"Your personal Information\" SubTitle=\"Information about your Identity.\">\n                <Group Title=\"Register\" SubTitle=\"Create a new Account on accounts.botech.dev\">\n                    <Stack Orientation=\"Vertical\">\n                        <Group Title=\"Personal Information\">\n                            <Stack Orientation=\"Horizontal\">\n                                <TextInput Name=\"First Name\" Property=\"personal.FirstName\"/>\n                                <TextInput Name=\"First Name\" Property=\"personal.FirstName\"/>\n                                <DateTimeInput Name=\"Birthday\" Property=\"personal.Birthday\" Value=\"01.01.0001 00:00:00\"/>\n                            </Stack>\n                        </Group>\n                        <Group Title=\"Address Information\">\n                            <Grid>\n                                <Row>\n                                    <Column>\n                                        <SearchTextInput StaticItemSource=\"../sources/boform/Countries.json\" Description=\"The country where you life in.\" Name=\"Country\" Property=\"address.Country\"/>\n                                    </Column>\n                                    <Column>\n                                        <SearchTextInput ItemSource=\"address.Cities\" SortBy=\"this.Country\" Description=\"The city where you life in.\" Name=\"City\" Property=\"address.City\"/>\n                                    </Column>\n                                </Row>\n                                <Row>\n                                    <Column>\n                                        <TextInput Name=\"Street\" Property=\"address.Country\"/>\n                                    </Column>\n                                    <Column>\n                                        <NumberInput Name=\"House Number\" Property=\"address.HouseNumber\" Value=\"0\" _gt=\"(_tId:1;_aTId:-1;_tn:ANumberInput`1;_nsp:BoTech.XmlParser.Tests.TestModels.Controls.Input.Numeric),(_tId:2;_aTId:1;_tn:Int32;_nsp:System)\"/>\n                                    </Column>\n                                    <Column>\n                                        <TextInput Name=\"Postal Code\" Property=\"address.PostalCode\"/>\n                                    </Column>\n                                </Row>\n                            </Grid>\n                        </Group>\n                    </Stack>\n                </Group>\n            </Step>\n            <Step Title=\"Just some statistics\" SubTitle=\"We are interested in your opinion.\">\n                <Group Title=\"Statistics\" SubTitle=\"We are interested in your opinion.\">\n                    <Stack Orientation=\"Horizontal\">\n                        <Input Name=\"Profession\" Property=\"statistic.Profession\"/>\n                        <StarInput Name=\"Just rate our services:\" Property=\"statistic.ProductRating\" Value=\"0\"/>\n                        <NumberInput Name=\"Years Of your Business Experience\" Property=\"statistic.YearsOfExperience\" Value=\"0\" _gt=\"(_tId:1;_aTId:-1;_tn:ANumberInput`1;_nsp:BoTech.XmlParser.Tests.TestModels.Controls.Input.Numeric),(_tId:2;_aTId:1;_tn:Int32;_nsp:System)\"/>\n                    </Stack>\n                </Group>\n            </Step>\n        </Form>\n    </BoForm.FormContent>\n    <BoForm.Imports>\n        <ViewModelImport NSP=\"BoTech.UI.Forms.Demo.DemoForm\" ClassName=\"PersonalInformation\" As=\"personal\" _nsp=\"BoTech.XmlParser.Tests.TestModels.Models.Import\"/>\n        <ViewModelImport NSP=\"BoTech.UI.Forms.Demo.DemoForm\" ClassName=\"ContactDetails\" As=\"contact\" _nsp=\"BoTech.XmlParser.Tests.TestModels.Models.Import\"/>\n        <ViewModelImport NSP=\"BoTech.UI.Forms.Demo.DemoForm\" ClassName=\"AddressDetails\" As=\"address\" _nsp=\"BoTech.XmlParser.Tests.TestModels.Models.Import\"/>\n        <ViewModelImport NSP=\"BoTech.UI.Forms.Demo.DemoForm\" ClassName=\"StatisticDetails\" As=\"statistic\" _nsp=\"BoTech.XmlParser.Tests.TestModels.Models.Import\"/>\n    </BoForm.Imports>\n</BoForm>\n";
    }

    [Test]
    public void TestDeserialize()
    {
        BoForm _deserializedSampleForm = new XmlDeserializer().Deserialize<BoForm>(_sampleFormAsXml);
    }
    [Test]
    public void TestGenericTypeParser()
    {
        TypeResolver.CreateInstance(typeof(TestDeserialization).Assembly);
        // Dictionary<List<int>, List<string>>
        GenericTypeInfo info = new GenericTypeParser().ParseGenericTypeFromXmlString(
            "(_tId:1;_aTId:-1;_tn:Dictionary`2;_nsp:System.Collections.Generic)," +
            "(_tId:2;_aTId:1;_tn:List`1;_nsp:System.Collections.Generic)," +
            "(_tId:3;_aTId:2;_tn:Int32;_nsp:System)," +
            "(_tId:4;_aTId:1;_tn:List`1;_nsp:System.Collections.Generic)," +
            "(_tId:5;_aTId:4;_tn:String;_nsp:System)");
        Console.WriteLine(info.ParseToString());
    }
}