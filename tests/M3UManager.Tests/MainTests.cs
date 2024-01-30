using M3UManager.Models;
using M3UManager.Tests.Utilities;

namespace M3UManager.Tests;

public class MainTests
{
    [SetUp]
    public void Setup()
    {

    }

    private static void ExtinfParseFromLinesTest(Stack<string> sampleM3UFileLines)
    {
        Channel channel = FindMethodUtility.CallPrivateStaticMethod<Channel>(typeof(M3UManager),
            "DetectChannelFromExtinfItem", sampleM3UFileLines);

        Assert.Multiple(() =>
        {
            Assert.That(channel.MediaUrl, Is.EqualTo("http://0.0.0.0/hbbh/stream.m3u8"));
            Assert.That(channel.Duration, Is.EqualTo("-1"));
            Assert.That(channel.Title, Is.EqualTo("HDTV (720p)"));
            Assert.That(channel.GroupTitle, Is.EqualTo("Undefined"));
            Assert.That(channel.TvgName, Is.EqualTo(null));
            Assert.That(channel.TvgID, Is.EqualTo("HDTV.fr"));
            Assert.That(channel.Logo, Is.EqualTo("https://y.imyr.cm/xy70wD.png"));
        });
    }

    [Test]
    public void AttributesExtinfParseFromStackStringTest()
        => ExtinfParseFromLinesTest(TestData.SampleAttributesExtinfLines);

    [Test]
    public void TagsExtinfParseFromStackStringTest()
        => ExtinfParseFromLinesTest(TestData.SampleTagsExtinfLines);

    // ******************************************************** //

    private static void ChannelToStringTest(Channel channel, string expectedChannelString, M3UType m3uType)
    {
        if (channel == null)
            throw new NullReferenceException($"'{nameof(channel)}' variable value is null.");

        string channelToStringValue = channel.ChannelToString(m3uType);

        Assert.That(channelToStringValue, Is.EqualTo(expectedChannelString));
    }

    [Test]
    public void AttributesExtinfToStringTest()
        => ChannelToStringTest(TestData.SampleAttributesExtinfChannel, TestData.SampleAttributesExtinfContent, M3UType.TagsType);

    [Test]
    public void TagsExtinfToStringTest()
        => ChannelToStringTest(TestData.SampleTagsExtinfChannel, TestData.SampleTagsExtinfContent, M3UType.AttributesType);
}