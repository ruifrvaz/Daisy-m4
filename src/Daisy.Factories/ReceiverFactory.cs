using Daisy.Resources.Interfaces;
using Daisy.Resources.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Daisy.Factories
{
    public sealed class ReceiverFactory
    {
        public static void LoadExternalReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IExternalReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(IExternalReceiver);

            // Load each configured receiver assembly individually
            foreach (var receiverName in settings.Receivers.Keys)
            {
                try
                {
                    Assembly assembly;
                    try
                    {
                        // Try to load using the assembly name first (this works for referenced assemblies)
                        assembly = Assembly.Load(receiverName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Fallback: try loading from file path
                        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{receiverName}.dll");
                        if (File.Exists(assemblyPath))
                        {
                            assembly = Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            continue; // Skip if assembly cannot be found
                        }
                    }

                    var assemblyName = assembly.GetName().Name!;
                    if (!settings.Receivers.ContainsKey(assemblyName))
                        continue;

                    var assemblySettings = settings.Receivers[assemblyName];
                    var receiverTypes = assembly.GetTypes()
                        .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass)
                        .ToList();

                    foreach (var receiverType in receiverTypes)
                    {
                        var receiver = (IExternalReceiver)Activator.CreateInstance(receiverType, assemblySettings.RunOnCores);
                        Resources.Pools.ExternalReceivers.Instance.Pool.Add(receiver);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue loading other assemblies
                    Console.WriteLine($"Warning: Could not load receiver assembly {receiverName}: {ex.Message}");
                }
            }
        }

        public static void LoadLoopBackReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement ILoopBackReceiver, create an instance for each of them
            // and load them into receiver pool
            var receiverInterface = typeof(ILoopBackReceiver);

            // Load each configured receiver assembly individually
            foreach (var receiverName in settings.Receivers.Keys)
            {
                try
                {
                    Assembly assembly;
                    try
                    {
                        // Try to load using the assembly name first (this works for referenced assemblies)
                        assembly = Assembly.Load(receiverName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Fallback: try loading from file path
                        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{receiverName}.dll");
                        if (File.Exists(assemblyPath))
                        {
                            assembly = Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            continue; // Skip if assembly cannot be found
                        }
                    }

                    var receiverTypes = assembly.GetTypes()
                        .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass)
                        .ToList();

                    foreach (var receiverType in receiverTypes)
                    {
                        // receivers may or may not implement ILoopBackReceiver receivers
                        var receiver = (ILoopBackReceiver)Activator.CreateInstance(receiverType);
                        Resources.Pools.LoopBackReceivers.Instance.Pool.Add(receiver);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue loading other assemblies
                    Console.WriteLine($"Warning: Could not load receiver assembly {receiverName}: {ex.Message}");
                }
            }
        }

        public static void LoadEventReceivers(ApplicationSettings settings, IServiceProvider serviceProvider)
        {
            // find all assemblies that implement IEventReceiver, create an instance for each of them and load them into receiver pool
            var receiverInterface = typeof(IEventReceiver);

            // Load each configured receiver assembly individually
            foreach (var receiverName in settings.Receivers.Keys)
            {
                try
                {
                    Assembly assembly;
                    try
                    {
                        // Try to load using the assembly name first (this works for referenced assemblies)
                        assembly = Assembly.Load(receiverName);
                    }
                    catch (FileNotFoundException)
                    {
                        // Fallback: try loading from file path
                        var assemblyPath = Path.Combine(AppContext.BaseDirectory, $"{receiverName}.dll");
                        if (File.Exists(assemblyPath))
                        {
                            assembly = Assembly.LoadFrom(assemblyPath);
                        }
                        else
                        {
                            continue; // Skip if assembly cannot be found
                        }
                    }

                    var assemblyName = assembly.GetName().Name!;
                    if (!settings.Receivers.ContainsKey(assemblyName))
                        continue;

                    var assemblySettings = settings.Receivers[assemblyName];
                    var receiverTypes = assembly.GetTypes()
                        .Where(type => receiverInterface.IsAssignableFrom(type) && type.IsClass)
                        .ToList();

                    foreach (var receiverType in receiverTypes)
                    {
                        // Subtle bug: assemblySettings could be null if the dictionary contains null values
                        // but we don't check for null before accessing RunOnCores property
                        // This could cause NullReferenceException in multithreaded scenarios
                        var runOnCores = assemblySettings.RunOnCores; // Potential null reference
                        var receiver = (IEventReceiver)Activator.CreateInstance(receiverType, runOnCores);
                        Resources.Pools.EventReceivers.Instance.Pool.Add(receiver);
                    }
                }
                catch (Exception ex)
                {
                    // Log the error but continue loading other assemblies
                    Console.WriteLine($"Warning: Could not load receiver assembly {receiverName}: {ex.Message}");
                }
            }
        }
    }
}
