using System.Text;
using Rocal.Core.Protocol;

namespace Rocal.Core.Tests;

public class EnvelopeTests
{
    private sealed record ChatPayload(string Text, DateTimeOffset SentAt);

    [Fact]
    public void RoundTrip_PreservesTypeIdAndPayload()
    {
        var id = Guid.NewGuid();
        var sentAt = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
        var bytes = EnvelopeSerializer.Serialize("chat.message", new ChatPayload("سلام", sentAt), id);

        var envelope = EnvelopeSerializer.Deserialize(bytes);

        Assert.Equal(ProtocolVersion.Current, envelope.Version);
        Assert.Equal("chat.message", envelope.Type);
        Assert.Equal(id, envelope.Id);
        Assert.Equal(new ChatPayload("سلام", sentAt), envelope.GetPayload<ChatPayload>());
    }

    [Fact]
    public void UnsupportedVersion_Throws()
    {
        var json = $$"""{"version":{{ProtocolVersion.Current + 1}},"type":"x","id":"{{Guid.NewGuid()}}"}""";
        var ex = Assert.Throws<UnsupportedProtocolVersionException>(() =>
            EnvelopeSerializer.Deserialize(Encoding.UTF8.GetBytes(json)));
        Assert.Equal(ProtocolVersion.Current + 1, ex.RequestedVersion);
    }

    [Fact]
    public void MissingVersion_Throws()
    {
        var json = $$"""{"type":"x","id":"{{Guid.NewGuid()}}"}""";
        Assert.Throws<UnsupportedProtocolVersionException>(() =>
            EnvelopeSerializer.Deserialize(Encoding.UTF8.GetBytes(json)));
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("")]
    [InlineData("{\"version\":1}")]
    [InlineData("[1,2,3]")]
    public void MalformedMessages_ThrowProtocolException(string json)
    {
        Assert.ThrowsAny<ProtocolException>(() => EnvelopeSerializer.Deserialize(Encoding.UTF8.GetBytes(json)));
    }

    [Fact]
    public void GetPayload_WithoutPayload_Throws()
    {
        var json = $$"""{"version":1,"type":"ping","id":"{{Guid.NewGuid()}}"}""";
        var envelope = EnvelopeSerializer.Deserialize(Encoding.UTF8.GetBytes(json));
        Assert.Throws<ProtocolException>(() => envelope.GetPayload<ChatPayload>());
    }

    [Fact]
    public void GetPayload_WithWrongShape_Throws()
    {
        var bytes = EnvelopeSerializer.Serialize("chat.message", new { Text = 5 });
        var envelope = EnvelopeSerializer.Deserialize(bytes);
        Assert.Throws<ProtocolException>(() => envelope.GetPayload<ChatPayload>());
    }
}
