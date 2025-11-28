using FluentHttpClient;

var client = new HttpClient();

var menu = await client
    .UsingRoute("https://www.w3schools.com/xml/simple.xml")
    .GetAsync()
    .ReadXmlAsync<BreakfastMenu>();

if (menu?.Food != null)
{
    Console.WriteLine("Menu Items:");
    foreach (var item in menu.Food)
    {
        Console.WriteLine($"- {item.Name}: {item.Price} ({item.Description})");
        Console.WriteLine($"  Calories: {item.Calories}");
    }
}

[System.Xml.Serialization.XmlRoot("breakfast_menu")]
public class BreakfastMenu
{
    [System.Xml.Serialization.XmlElement("food")]
    public List<FoodItem>? Food { get; set; }
}

public class FoodItem
{
    [System.Xml.Serialization.XmlElement("name")]
    public string? Name { get; set; }
    
    [System.Xml.Serialization.XmlElement("price")]
    public string? Price { get; set; }
    
    [System.Xml.Serialization.XmlElement("description")]
    public string? Description { get; set; }
    
    [System.Xml.Serialization.XmlElement("calories")]
    public int Calories { get; set; }
}
