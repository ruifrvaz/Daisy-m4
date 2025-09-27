using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using Daisy.Resources.Pools;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Daisy.Factories
{
    public sealed class TransmitterFactory
    {
        public static void LoadExternalTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // look in assemblies and find all classes that implement IExternalTransmitter
            var transmitterInterface = typeof(IExternalTransmitter);

            // Load each configured transmitter assembly individually
            foreach (var transmitterName in settings.Transmitters)
            {
                try
                {
                    Assembly assembly;
                    try
                    {
                        // Try to load using the assembly name first (this works for referenced assemblies)
                        assembly = Assembly.Load(transmitterName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Fallback: try loading from file path
                        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{transmitterName}.dll");
                        if (File.Exists(assemblyPath))
                        {
                            assembly = Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            continue; // Skip if assembly cannot be found
                        }
                    }

                    var transmitterTypes = assembly.GetTypes()
                        .Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass)
                        .ToList();

                    foreach (var transmitterType in transmitterTypes)
                    {
                        // transmitters may or may not implement external IExternalTransmitter
                        if (transmitterType != null)
                        {
                            var transmitter = (IExternalTransmitter)Activator.CreateInstance(transmitterType);
                            ExternalTransmitters.Instance.Pool.Add(transmitter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue loading other assemblies
                    Console.WriteLine($"Warning: Could not load transmitter assembly {transmitterName}: {ex.Message}");
                }
            }
        }

        public static void LoadLoopBackTransmitters(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // look in assemblies and find all classes that implement ILoopBackTransmitter
            var transmitterInterface = typeof(ILoopBackTransmitter);

            // Load each configured transmitter assembly individually
            foreach (var transmitterName in settings.Transmitters)
            {
                try
                {
                    Assembly assembly;
                    try
                    {
                        // Try to load using the assembly name first (this works for referenced assemblies)
                        assembly = Assembly.Load(transmitterName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Fallback: try loading from file path
                        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{transmitterName}.dll");
                        if (File.Exists(assemblyPath))
                        {
                            assembly = Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            continue; // Skip if assembly cannot be found
                        }
                    }

                    var transmitterTypes = assembly.GetTypes()
                        .Where(type => transmitterInterface.IsAssignableFrom(type) && type.IsClass)
                        .ToList();

                    foreach (var transmitterType in transmitterTypes)
                    {
                        // transmitters may or may not implement external ILoopBackTransmitter
                        if (transmitterType != null)
                        {
                            var transmitter = (ILoopBackTransmitter)Activator.CreateInstance(transmitterType);
                            LoopBackTransmitters.Instance.Pool.Add(transmitter);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue loading other assemblies
                    Console.WriteLine($"Warning: Could not load transmitter assembly {transmitterName}: {ex.Message}");
                }
            }
        }
    }
}