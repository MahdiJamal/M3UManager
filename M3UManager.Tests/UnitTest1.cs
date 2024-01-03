using Xunit;

namespace M3UManager.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Sho()
        {
            var sdf = M3UManager.Parse(@".\Datas\test.m3u");
            var dsffgff = sdf.GetM3UAsString();

        }
    }
}