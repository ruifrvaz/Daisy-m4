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

            switch (field)
            {
                case ImpulseField.Input:
                    impulse.Input = string.IsNullOrEmpty(impulse.Input) ? formatted : impulse.Input + formatted;
                    break;
                case ImpulseField.Output:
                    impulse.Output = string.IsNullOrEmpty(impulse.Output) ? formatted : impulse.Output + formatted;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(field), field, null);
            }
        }

        public static string GetChainByKey(this Impulse impulse, string key, ImpulseField field = ImpulseField.Input)
        {
            if (impulse == null)
            {
                throw new ArgumentNullException(nameof(impulse));
            }

            string source = field switch
            {
                ImpulseField.Input => impulse.Input,
                ImpulseField.Output => impulse.Output,
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
            };

            return source.GetChainByKey(key);
        }
    }
}
