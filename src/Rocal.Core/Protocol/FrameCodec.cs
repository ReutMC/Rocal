using System.Buffers.Binary;

namespace Rocal.Core.Protocol;

/// <summary>
/// Length-prefixed framing: [4-byte big-endian payload length][payload].
/// Handles partial reads, clean end-of-stream, truncated frames and oversized frames.
/// </summary>
public static class FrameCodec
{
    public const int HeaderSize = 4;

    /// <summary>Upper bound for a single control/chat frame. Bulk data uses its own chunked framing.</summary>
    public const int DefaultMaxFrameSize = 16 * 1024 * 1024;

    public static async ValueTask WriteFrameAsync(
        Stream stream,
        ReadOnlyMemory<byte> payload,
        CancellationToken cancellationToken = default,
        int maxFrameSize = DefaultMaxFrameSize)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (payload.Length > maxFrameSize)
            throw new ProtocolException($"Frame of {payload.Length} bytes exceeds the {maxFrameSize}-byte limit.");

        var header = new byte[HeaderSize];
        BinaryPrimitives.WriteInt32BigEndian(header, payload.Length);
        await stream.WriteAsync(header, cancellationToken).ConfigureAwait(false);
        await stream.WriteAsync(payload, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads one frame. Returns <c>null</c> if the stream ended cleanly before any header byte arrived.
    /// </summary>
    public static async ValueTask<byte[]?> ReadFrameAsync(
        Stream stream,
        CancellationToken cancellationToken = default,
        int maxFrameSize = DefaultMaxFrameSize)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var header = new byte[HeaderSize];
        int headerRead = await ReadFullyAsync(stream, header, cancellationToken).ConfigureAwait(false);
        if (headerRead == 0) return null;
        if (headerRead < HeaderSize) throw new ProtocolException("Connection closed in the middle of a frame header.");

        int length = BinaryPrimitives.ReadInt32BigEndian(header);
        if (length < 0 || length > maxFrameSize)
            throw new ProtocolException($"Invalid frame length {length}.");

        var payload = new byte[length];
        int bodyRead = await ReadFullyAsync(stream, payload, cancellationToken).ConfigureAwait(false);
        if (bodyRead < length) throw new ProtocolException("Connection closed in the middle of a frame body.");
        return payload;
    }

    private static async ValueTask<int> ReadFullyAsync(Stream stream, Memory<byte> buffer, CancellationToken ct)
    {
        int total = 0;
        while (total < buffer.Length)
        {
            int n = await stream.ReadAsync(buffer[total..], ct).ConfigureAwait(false);
            if (n == 0) break;
            total += n;
        }
        return total;
    }
}
