using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        var settings = new 
        {
            Abilities = new[] { 
                "Daisy.Abilities.Operator",
                "Daisy.Abilities.Terminate", 
                "Daisy.Abilities.OutputValidator", 
                "Daisy.Abilities.Weather" 
            },
            Receivers = new[] {
                "Daisy.Receivers.Console",
                "Daisy.Receivers.WeatherEvent"
            }
        };
        
        Console.WriteLine("All loaded assemblies:");
        var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in allAssemblies)
        {
            Console.WriteLine($"  {assembly.GetName().Name}");
        }
        
        Console.WriteLine("\nDaisy assemblies:");
        var daisyAssemblies = allAssemblies
            .Where(assembly => assembly.GetName().Name.StartsWith("Daisy."))
            .ToList();
        foreach (var assembly in daisyAssemblies)
        {
            Console.WriteLine($"  {assembly.GetName().Name}");
        }
        
        Console.WriteLine("\nAbility assemblies from config:");
        var abilityAssemblies = allAssemblies
            .Where(assembly => settings.Abilities.Any(abilityName =>
                assembly.GetName().Name.Equals(abilityName, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        foreach (var assembly in abilityAssemblies)
        {
            Console.WriteLine($"  {assembly.GetName().Name}");
        }
        
        Console.WriteLine("\nReceiver assemblies from config:");
        var receiverAssemblies = allAssemblies
            .Where(assembly => settings.Receivers.Any(receiverName =>
                assembly.GetName().Name.Equals(receiverName, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        foreach (var assembly in receiverAssemblies)
        {
            Console.WriteLine($"  {assembly.GetName().Name}");
        }
    }
}