namespace TarkovCheat.Core.Tests;

using Xunit;
using TarkovCheat.Core.Memory;

public class PatternScannerTests
{
    [Fact]
    public void Scan_ExactMatch_ReturnsOffset()
    {
        byte[] region = { 0x00, 0x48, 0x8B, 0x05, 0xAA, 0xBB, 0xCC, 0xDD, 0x00 };
        nint result = PatternScanner.Scan(region, "48 8B 05 ?? ?? ?? ??");
        Assert.Equal((nint)1, result);
    }

    [Fact]
    public void Scan_NoMatch_ReturnsZero()
    {
        byte[] region = { 0x00, 0x01, 0x02, 0x03, 0x04 };
        nint result = PatternScanner.Scan(region, "FF EE DD");
        Assert.Equal(nint.Zero, result);
    }

    [Fact]
    public void Scan_WildcardsOnly_MatchesFirstPosition()
    {
        byte[] region = { 0xAA, 0xBB, 0xCC };
        nint result = PatternScanner.Scan(region, "?? ??");
        Assert.Equal(nint.Zero, result);
    }

    [Fact]
    public void Scan_PatternAtEnd_ReturnsCorrectOffset()
    {
        byte[] region = { 0x00, 0x00, 0x00, 0xDE, 0xAD, 0xBE, 0xEF };
        nint result = PatternScanner.Scan(region, "DE AD BE EF");
        Assert.Equal((nint)3, result);
    }

    [Fact]
    public void Scan_EmptyRegion_ReturnsZero()
    {
        nint result = PatternScanner.Scan(Array.Empty<byte>(), "AA BB");
        Assert.Equal(nint.Zero, result);
    }

    [Fact]
    public void Scan_SingleByte_FindsFirst()
    {
        byte[] region = { 0x10, 0x20, 0x30, 0x20 };
        nint result = PatternScanner.Scan(region, "20");
        Assert.Equal((nint)1, result);
    }

    [Fact]
    public void ScanAll_MultipleMatches_ReturnsAll()
    {
        byte[] region = { 0xAA, 0x00, 0xAA, 0x00, 0xAA };
        var results = PatternScanner.ScanAll(region, "AA");
        Assert.Equal(3, results.Count);
        Assert.Equal((nint)0, results[0]);
        Assert.Equal((nint)2, results[1]);
        Assert.Equal((nint)4, results[2]);
    }
}
