// See https://aka.ms/new-console-template for more information


using System.Reflection;

var className = new ClassName();

var property = typeof(ClassName).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .FirstOrDefault();

System.Console.WriteLine(property.CanWrite);

public class ClassName
{
    public int X { get; }

}