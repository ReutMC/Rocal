using Rocal.Core.Protocol;

namespace Rocal.Core.Tests;

public class FrameCodecTests
{
    /// <summary>Returns at most one byte per read, simulating worst-case TCP segmentation.</summary>
    private sealed class OneByteAtATimeStream(Stream inner) : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => inner.Read(buffer, offset, Math.Min(1, count));
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }

    private static async Task<MemoryStream> EncodeAsync(byte[] payload)
    {
        var ms = new MemoryStream();
        await FrameCodec.WriteFrameAsync(ms, payload);
        ms.Position = 0;
        return ms;
    }

    [Fact]
    public async Task RoundTrip_ReturnsSamePayload()
    {
        var payload = new byte[] { 1, 2, 3, 4, 5 };
        using var ms = await EncodeAsync(payload);
        Assert.Equal(payload, await FrameCodec.ReadFrameAsync(ms));
    }

    [Fact]
    public async Task EmptyPayload_RoundTrips()
    {
        using var ms = await EncodeAsync([]);
        Assert.Empty((await FrameCodec.ReadFrameAsync(ms))!);
    }

    [Fact]
    public async Task PartialReads_AreReassembled()
    {
        var payload = Enumerable.Range(0, 300).Select(i => (byte)i).ToArray();
        using var slow = new OneByteAtATimeStream(await EncodeAsync(payload));
        Assert.Equal(payload, await FrameCodec.ReadFrameAsync(slow));
    }

    [Fact]
    public async Task MultipleFrames_AreReadInOrder()
    {
        using var ms = new MemoryStream();
        await FrameCodec.WriteFrameAsync(ms, new byte[] { 1 });
        await FrameCodec.WriteFrameAsync(ms, new byte[] { 2, 2 });
        ms.Position = 0;
        Assert.Equal(new byte[] { 1 }, await FrameCodec.ReadFrameAsync(ms));
        Assert.Equal(new byte[] { 2, 2 }, await FrameCodec.ReadFrameAsync(ms));
        Assert.Null(await FrameCodec.ReadFrameAsync(ms));
    }

    [Fact]
    public async Task CleanEndOfStream_ReturnsNull()
    {
        using var ms = new MemoryStream();
        Assert.Null(await FrameCodec.ReadFrameAsync(ms));
    }

    [Fact]
    public async Task TruncatedHeader_Throws()
    {
        using var ms = new MemoryStream(new byte[] { 0, 0 });
        await Assert.ThrowsAsync<ProtocolException>(async () => await FrameCodec.ReadFrameAsync(ms));
    }

    [Fact]
    public async Task TruncatedBody_Throws()
    {
        using var ms = new MemoryStream(new byte[] { 0, 0, 0, 10, 1, 2, 3 });
        await Assert.ThrowsAsync<ProtocolException>(async () => await FrameCodec.ReadFrameAsync(ms));
    }

    [Theory]
    [InlineData(new byte[] { 0x7F, 0xFF, 0xFF, 0xFF })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })] // negative length
    public async Task InvalidLength_Throws(byte[] header)
    {
        using var ms = new MemoryStream(header);
        await Assert.ThrowsAsync<ProtocolException>(async () => await FrameCodec.ReadFrameAsync(ms));
    }

    [Fact]
    public async Task Write_OversizedPayload_Throws()
    {
        using var ms = new MemoryStream();
        await Assert.ThrowsAsync<ProtocolException>(async () =>
            await FrameCodec.WriteFrameAsync(ms, new byte[11], default, maxFrameSize: 10));
    }

    [Fact]
    public async Task Read_HonoursCustomMaxFrameSize()
    {
        using var ms = await EncodeAsync(new byte[100]);
        await Assert.ThrowsAsync<ProtocolException>(async () =>
            await FrameCodec.ReadFrameAsync(ms, default, maxFrameSize: 50));
    }
}
