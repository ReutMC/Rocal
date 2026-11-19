namespace Rocal.Core.Protocol;

/// <summary>Raised when data received from a peer violates the Rocal wire protocol.</summary>
public class ProtocolException : Exception
{
    public ProtocolException(string message) : base(message) { }
    public ProtocolException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>Raised when a peer speaks a protocol version this build does not support.</summary>
public sealed class UnsupportedProtocolVersionException : ProtocolException
{
    public UnsupportedProtocolVersionException(int requestedVersion)
        : base($"Unsupported protocol version {requestedVersion}.")
    {
        RequestedVersion = requestedVersion;
    }

    public int RequestedVersion { get; }
}
