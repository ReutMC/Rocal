using System.Text.Json;

namespace Rocal.Core.Protocol;

public static class ProtocolVersion
{
    public const int Current = 1;
}

public static class ProtocolJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}

/// <summary>Versioned message wrapper. The payload shape is determined by <see cref="Type"/>.</summary>
public sealed record Envelope(int Version, string Type, Guid Id, JsonElement? Payload)
{
    public T GetPayload<T>()
    {
        if (Payload is null) throw new ProtocolException($"Message '{Type}' has no payload.");
        try
        {
            return Payload.Value.Deserialize<T>(ProtocolJson.Options)
                   ?? throw new ProtocolException($"Message '{Type}' has an empty payload.");
        }
        catch (JsonException ex)
        {
            throw new ProtocolException($"Message '{Type}' has an invalid payload.", ex);
        }
    }
}

public static class EnvelopeSerializer
{
    public static byte[] Serialize<T>(string type, T payload, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(type)) throw new ArgumentException("Message type is required.", nameof(type));
        var envelope = new Envelope(
            ProtocolVersion.Current,
            type,
            id ?? Guid.NewGuid(),
            JsonSerializer.SerializeToElement(payload, ProtocolJson.Options));
        return JsonSerializer.SerializeToUtf8Bytes(envelope, ProtocolJson.Options);
    }

    public static Envelope Deserialize(ReadOnlySpan<byte> data)
    {
        Envelope? envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<Envelope>(data, ProtocolJson.Options);
        }
        catch (JsonException ex)
        {
            throw new ProtocolException("Malformed message.", ex);
        }

        if (envelope is null || string.IsNullOrWhiteSpace(envelope.Type))
            throw new ProtocolException("Message is missing its type.");
        if (envelope.Version < 1 || envelope.Version > ProtocolVersion.Current)
            throw new UnsupportedProtocolVersionException(envelope.Version);
        return envelope;
    }
}
