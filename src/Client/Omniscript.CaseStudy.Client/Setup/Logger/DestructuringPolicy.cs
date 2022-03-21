using System.Text.Json;

using MediatR;

using Serilog.Core;
using Serilog.Events;

namespace Omniscript.CaseStudy.Client.Setup.Logger
{
    internal sealed class DestructuringPolicy : IDestructuringPolicy
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public bool TryDestructure(
            object value,
            ILogEventPropertyValueFactory propertyValueFactory,
            out LogEventPropertyValue? result)
        {
            if (value is IBaseRequest)
            {
                var requestSerialized = JsonSerializer.Serialize(
                    value,
                    JsonSerializerOptions);
                result = new ScalarValue(requestSerialized);

                return true;
            }
            else
            {
                result = null;

                return false;
            }
        }
    }
}