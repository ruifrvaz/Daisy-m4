using Daisy.Resources.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Daisy.Resources.Services
{
    public sealed class ServiceContainer
    {
        public List<IDaisyService> Services { get; set; }

        private static ServiceProvider _serviceProvider;

        private static ServiceContainer _instance;

        private ServiceContainer()
        {
            Services = new List<IDaisyService>();
        }

        public static ServiceContainer Instance
        {
            get
            {
                return _instance ?? (_instance = new ServiceContainer());
            }
        }

        public IDaisyService GetService<T>() where T : IDaisyService
        {
            return Instance.Services.FirstOrDefault(s => typeof(T).IsAssignableFrom(s.GetType()));
        }

        public void AddServiceProvider(ServiceProvider serviceProvider)
        {
            _serviceProvider ??= serviceProvider;
        }

        public ServiceProvider GetServiceProvider()
        {
            if (_serviceProvider == null)
            {
                throw new Exception("Service provider not initialized.");
            };

            return _serviceProvider;
        }

        public void CleanupServiceProvider()
        {
            if (_serviceProvider != null)
            {
                foreach (var disposable in Services.OfType<IDisposable>())
                {
                    disposable.Dispose();
                }
                Services.Clear();

                _serviceProvider.Dispose();
                _serviceProvider = null;
            };
        }
    }
}
