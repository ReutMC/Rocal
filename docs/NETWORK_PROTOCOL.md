# Rocal network protocol

Status: **framing and envelope implemented** (`Rocal.Core.Protocol`). Discovery, pairing, transfer,
chat and remote-desktop messages are specified here as they are built.

## Framing

Every control message on a TCP/TLS stream is one frame:

| Field   | Size    | Notes                          |
|---------|---------|--------------------------------|
| length  | 4 bytes | big-endian, payload length     |
| payload | N bytes | UTF-8 JSON `Envelope`          |

- Maximum frame size: 16 MiB (`FrameCodec.DefaultMaxFrameSize`). Larger or negative lengths are a protocol error.
- A clean end-of-stream before the first header byte is a normal disconnect (`ReadFrameAsync` returns `null`).
- End-of-stream inside a header or body is a `ProtocolException`.
- Partial reads are reassembled (covered by tests that deliver one byte at a time).
- Bulk file data and screen frames will use separate connections with their own chunked framing, so
  they cannot stall control traffic.

## Envelope

```json
{ "version": 1, "type": "chat.message", "id": "<guid>", "payload": { } }
```

- `version`: protocol version. Messages with a missing, zero or newer-than-supported version are rejected
  with `UnsupportedProtocolVersionException`.
- `type`: message type string, determines the payload shape.
- `id`: GUID used for de-duplication and request/response correlation.
- Malformed JSON, a missing type, or a payload of the wrong shape raise `ProtocolException`.
