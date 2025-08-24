using System;

namespace Daisy.Resources.Interfaces
{
    /// <summary>
    /// Identifier of Daisy services
    /// </summary>
    public interface IDaisyService
    {
        public void Initialize(IServiceProvider serviceProvider);
    }
}