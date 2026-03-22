using System.Reflection;
using System.Xml.Serialization;
using BoTech.XmlParser.Error.ObjectDefinitions;
using BoTech.XmlParser.Tests.TestModels.Controls;
using BoTech.XmlParser.Tests.TestModels.Controls.Input;
using BoTech.XmlParser.Tests.TestModels.Controls.Input.DateTime;
using BoTech.XmlParser.Tests.TestModels.Controls.Input.Numeric;
using BoTech.XmlParser.Tests.TestModels.Controls.Input.Text;
using BoTech.XmlParser.Tests.TestModels.Controls.Layout;
using BoTech.XmlParser.Tests.TestModels.Controls.Layout.Grid;
using BoTech.XmlParser.Tests.TestModels.Models.Import;

namespace BoTech.XmlParser.Tests;

public class TestSerialization
{
    private BoForm _sampleForm = new BoForm();
    [SetUp]
    public void Setup()
    {
        _sampleForm =new BoForm()
        {
            Imports =
            {
                new ViewModelImport()
                {
                    Namespace = "BoTech.UI.Forms.Demo.DemoForm",
                    ClassName = "PersonalInformation",
                    As = "personal"
                },
                new ViewModelImport()
                {
                    Namespace = "BoTech.UI.Forms.Demo.DemoForm",
                    ClassName = "ContactDetails",
                    As = "contact"
                },
                new ViewModelImport()
                {
                    Namespace = "BoTech.UI.Forms.Demo.DemoForm",
                    ClassName = "AddressDetails",
                    As = "address"
                },
                new ViewModelImport()
                {
                    Namespace = "BoTech.UI.Forms.Demo.DemoForm",
                    ClassName = "StatisticDetails",
                    As = "statistic"
                }
            },
            FormContent = new Form()
            {
                IsStepped = true,
                Content =
                {
                    new AStep()
                    {
                        Title = "Your personal Information",
                        SubTitle = "Information about your Identity.",
                        Content = new AGroup()
                        {
                            Title = "Register",
                            SubTitle = "Create a new Account on accounts.botech.dev",
                            Content = new AStack()
                            {
                                Orientation = Orientation.Vertical,
                                Children= new List<IFormElement>()
                                {
                                    new AGroup()
                                    {
                                        Title = "Personal Information",
                                        Content = new AStack()
                                        {
                                            Orientation = Orientation.Horizontal,
                                            Children = new List<IFormElement>()
                                            {
                                                new ATextInput()
                                                {
                                                    Name = "First Name",
                                                    Property = "personal.FirstName"
                                                },
                                                new ATextInput()
                                                {
                                                    Name = "First Name",
                                                    Property = "personal.FirstName"
                                                },
                                                new ADateTimeInput()
                                                {
                                                    Name = "Birthday",
                                                    Property = "personal.Birthday"
                                                }
                                            }
                                        }
                                    },
                                    new AGroup()
                                    {
                                        Title = "Address Information",
                                        Content = new AGrid()
                                        {
                                            Rows =
                                            {
                                                new ARow()
                                                {
                                                    Columns =
                                                    {
                                                        new AColumn()
                                                        {
                                                            Content = new ASearchTextInput()
                                                            {
                                                                Name ="Country",
                                                                Description="The country where you life in.",
                                                                StaticItemSource="../sources/boform/Countries.json",
                                                                Property="address.Country"
                                                            }
                                                        },
                                                        new AColumn()
                                                        {
                                                            Content = new ASearchTextInput()
                                                            {
                                                                Name="City",
                                                                Description="The city where you life in.",
                                                                SortBy="this.Country",
                                                                ItemSource="address.Cities",
                                                                Property="address.City"
                                                            }
                                                        }
                                                    }
                                                },
                                                new ARow()
                                                {
                                                    Columns =
                                                    {
                                                        new AColumn()
                                                        {
                                                            Content = new ATextInput()
                                                            {
                                                                Name ="Street",
                                                                Property="address.Country"
                                                            }
                                                        },
                                                        new AColumn()
                                                        {
                                                            Content = new ANumberInput<int>()
                                                            {
                                                                Name ="House Number",
                                                                Property="address.HouseNumber"
                                                            }
                                                        },
                                                        new AColumn()
                                                        {
                                                            Content = new ATextInput()
                                                            {
                                                                Name ="Postal Code",
                                                                Property="address.PostalCode"
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    } 
                                }
                            }
                        }
                    },
                    new AStep()
                    {
                        Title="Just some statistics",
                        SubTitle="We are interested in your opinion.",
                        Content = new AGroup()
                        {
                            Title = "Statistics",
                            SubTitle = "We are interested in your opinion.",
                            Content = new AStack()
                            {
                                Orientation = Orientation.Horizontal,
                                Children = new List<IFormElement>()
                                {
                                    new AInput()
                                    {
                                        Name="Profession",
                                        Property="statistic.Profession"
                                    },
                                    new AStarInput()
                                    {
                                        Name="Just rate our services:",
                                        Property="statistic.ProductRating"
                                    },
                                    new ANumberInput<int>()
                                    {
                                        Name="Years Of your Business Experience",
                                        Property="statistic.YearsOfExperience"
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };
    }
    [Test]
    public void SerializationTest()
    {
        string xml = new XmlSerializer().Serialize(_sampleForm);
        File.WriteAllText(@"C:\temp\boform.xml", xml);
    }
}