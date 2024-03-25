using System.Text.Json.Serialization;

namespace Bookery.Node.Common.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccessTypeId
{
    Read = 0,
    Write = 1
}