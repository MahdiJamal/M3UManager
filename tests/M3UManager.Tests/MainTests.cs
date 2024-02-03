using M3UManager.Models;
using M3UManager.Tests.Utilities;

namespace M3UManager.Tests;

public class MainTests
{
    [SetUp]
    public void Setup()
    {

    }

    [Test]
    public void M3UFullParseFromLinesTest()
    {
        M3U parsedM3U = M3UManager.ParseFromLines(TestData.SampleM3ULinesList);

        Assert.Multiple(() =>
        {
            Assert.That(parsedM3U, Is.Not.EqualTo(null));
            Assert.That(parsedM3U.Channels, Is.Not.EqualTo(null));
            Assert.That(parsedM3U.Channels, Has.Count.EqualTo(2));
            Assert.That(parsedM3U.HasEndList, Is.EqualTo(false));
            Assert.That(parsedM3U.PlayListType, Is.EqualTo(null));
            Assert.That(parsedM3U.MediaSequence, Is.EqualTo(null));
            Assert.That(parsedM3U.TargetDuration, Is.EqualTo(null));
            Assert.That(parsedM3U.Version, Is.EqualTo(null));

            Assert.That(parsedM3U.Channels[0].MediaUrl, Is.EqualTo("http://0.0.0.0/hbbh/stream.m3u8"));
            Assert.That(parsedM3U.Channels[0].Duration, Is.EqualTo("-1"));
            Assert.That(parsedM3U.Channels[0].Title, Is.EqualTo("HDTV (720p) [Not 24/7]"));
            Assert.That(parsedM3U.Channels[0].GroupTitle, Is.EqualTo("Undefined"));
            Assert.That(parsedM3U.Channels[0].TvgName, Is.EqualTo(null));
            Assert.That(parsedM3U.Channels[0].TvgID, Is.EqualTo("HDTV.fr"));
            Assert.That(parsedM3U.Channels[0].Logo, Is.EqualTo("https://o.imur.om/xyW0wD.png"));

            Assert.That(parsedM3U.Channels[1].MediaUrl, Is.EqualTo("http://0.0.0.0/hbbh/stream.m3u8"));
            Assert.That(parsedM3U.Channels[1].Duration, Is.EqualTo("-1"));
            Assert.That(parsedM3U.Channels[1].Title, Is.EqualTo("HDTV (720p)"));
            Assert.That(parsedM3U.Channels[1].GroupTitle, Is.EqualTo("Undefined"));
            Assert.That(parsedM3U.Channels[1].TvgName, Is.EqualTo(null));
            Assert.That(parsedM3U.Channels[1].TvgID, Is.EqualTo("HDTV.fr"));
            Assert.That(parsedM3U.Channels[1].Logo, Is.EqualTo("https://y.imyr.cm/xy70wD.png"));
        });
    }

    // ******************************************************** //

    private static void ExtinfParseFromLinesTest(Stack<string> sampleExtinfLines)
    {
        Channel channel = FindMethodUtility.CallPrivateStaticMethod<Channel>(typeof(M3UManager),
            "DetectChannelFromExtinfItem", sampleExtinfLines);

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
        ArgumentNullException.ThrowIfNull(nameof(channel), $"'{nameof(channel)}' variable value is null.");

        string channelToStringValue = channel.ToString(m3uType);

        Assert.That(channelToStringValue, Is.EqualTo(expectedChannelString));
    }

    [Test]
    public void AttributesExtinfToStringTest()
        => ChannelToStringTest(TestData.SampleAttributesExtinfChannel, TestData.SampleAttributesExtinfContent, M3UType.TagsType);

    [Test]
    public void TagsExtinfToStringTest()
        => ChannelToStringTest(TestData.SampleTagsExtinfChannel, TestData.SampleTagsExtinfContent, M3UType.AttributesType);
}