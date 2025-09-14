using Daisy.Resources.Signals;
using System;

namespace Daisy.Resources.Extensions
{
    public static class ImpulseExtensions
    {
        public enum ImpulseField
        {
            Input,
            Output
        }

        public static void AddChain(this Impulse impulse, string chain, ImpulseField field = ImpulseField.Input)
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

            if (field == ImpulseField.Input)
            {
                impulse.Input = string.IsNullOrEmpty(impulse.Input) ? formatted : impulse.Input + formatted;
            }
            else
            {
                impulse.Output = string.IsNullOrEmpty(impulse.Output) ? formatted : impulse.Output + formatted;
            }
        }

        public static string GetChainByKey(this Impulse impulse, string key, ImpulseField field = ImpulseField.Input)
        {
            if (impulse == null)
            {
                throw new ArgumentNullException(nameof(impulse));
            }

            var source = field == ImpulseField.Input ? impulse.Input : impulse.Output;

            return source.GetChainByKey(key);
        }
    }
}
