![BoTech_Logo](https://assets.botech.dev/Logos/BoTechLogoCompleteWithSlogan.png "The official Logo of BoTech with the slogan.")
# BoTech.XmlParser

+ This Xml Parser is a custom xml Parser that supports generics, interfaces and abstract classes.
+ The Xml Parser uses internal XmlProperties to achieve this. `_gt` for generics and `_nsp` for specific namespaces.

## Installation
+ Just install the following package:
```
dotnet add package BoTech.XmlParser
```
## Examples usage:
### Serialization
```csharp
// Add missing usings here.
namespace TestSerialization
{
  public class Program
  {
    public static void Main(string[] args)
    {
        string xml = new XmlSerializer().Serialize(new Employee<int>("First Name, Last Name", 3000));
        Console.WriteLine(xml);   
    }
  }
  [XmlName("Employee")]
  public class Employee<T>(string name, int salary)
  {
     public string Name {get; set;} = name;
     public T Salary {get; set;} = salary;
  } 
}
```
#### Result
```xml
<?xml version="1.0" encoding="UTF-8"?>
<Employee Name="First Name, Last Name" Salary="3000" _gt="(_tId:1;_aTId:-1;_tn:Employee`1;_nsp:BoTech.XmlParser.Tests),(_tId:2;_aTId:1;_tn:Int32;_nsp:System)"/>
```

### Deserialization
#### Xml to deserialize

```xml
<?xml version="1.0" encoding="UTF-8"?>
<Employee Name="First Name, Last Name" Salary="3000" _gt="(_tId:1;_aTId:-1;_tn:Employee`1;_nsp:BoTech.XmlParser.Tests),(_tId:2;_aTId:1;_tn:Int32;_nsp:System)"/>
```
#### Code:
```csharp
// Add missing usings here.
namespace TestSerialization
{
  public class Program
  {
    public static void Main(string[] args)
    {
        Employee<int> employee = (Employee<int>)new XmlDeserializer().Deserialize("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Employee Name=\"First Name, Last Name\" Salary=\"3000\" _gt=\"(_tId:1;_aTId:-1;_tn:Employee`1;_nsp:BoTech.XmlParser.Tests),(_tId:2;_aTId:1;_tn:Int32;_nsp:System)\"/>");
        Console.WriteLine(employee.Salary); 
    }
  }
  [XmlName("Employee")]
  public class Employee<T>
  {
     public string Name {get; set;}
     public T Salary {get; set;}
  } 
}
```

