using Daisy.Resources.Signals;
using System;

namespace Daisy.Resources.Extensions
{
    public static class ImpulseExtensions
    {
        public static void AddChain(this Impulse impulse, string chain)
        {
            if (impulse == null)
            {
                throw new ArgumentNullException(nameof(impulse));
            }

            if (string.IsNullOrWhiteSpace(chain))
            {
                return;
            }

            var formatted = $"<{chain}>";
            impulse.Input = string.IsNullOrEmpty(impulse.Input) ? formatted : impulse.Input + formatted;
        }
    }
}
