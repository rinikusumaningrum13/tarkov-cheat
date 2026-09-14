namespace TarkovCheat.Core.Tests;

using Xunit;
using TarkovCheat.Core.Config;

public class SettingsTests
{
    [Fact]
    public void Defaults_AimFov_IsFive()
    {
        var s = new CheatSettings();
        Assert.Equal(5.0f, s.Aim.Fov);
    }

    [Fact]
    public void Defaults_EspEnabled()
    {
        var s = new CheatSettings();
        Assert.True(s.Esp.Enabled);
        Assert.True(s.Esp.Box);
        Assert.True(s.Esp.Health);
        Assert.False(s.Esp.Skeleton);
    }

    [Fact]
    public void Defaults_TriggerDisabled()
    {
        var s = new CheatSettings();
        Assert.False(s.Trigger.Enabled);
        Assert.True(s.Trigger.TeamCheck);
        Assert.Equal(50, s.Trigger.DelayMs);
    }

    [Fact]
    public void RoundTrip_Json_PreservesValues()
    {
        var original = new CheatSettings();
        original.Aim.Fov = 8.5f;
        original.Aim.Smooth = 2.0f;
        original.Esp.Skeleton = true;
        original.Trigger.DelayMs = 75;

        string json = original.ToJson();
        var restored = CheatSettings.FromJson(json);

        Assert.Equal(8.5f, restored.Aim.Fov);
        Assert.Equal(2.0f, restored.Aim.Smooth);
        Assert.True(restored.Esp.Skeleton);
        Assert.Equal(75, restored.Trigger.DelayMs);
    }

    [Fact]
    public void Clone_ReturnsIndependentCopy()
    {
        var original = new CheatSettings();
        original.Aim.Fov = 10.0f;

        var clone = original.Clone();
        clone.Aim.Fov = 3.0f;

        Assert.Equal(10.0f, original.Aim.Fov);
        Assert.Equal(3.0f, clone.Aim.Fov);
    }

    [Fact]
    public void FromJson_Empty_ReturnsDefaults()
    {
        var s = CheatSettings.FromJson("{}");
        Assert.Equal(5.0f, s.Aim.Fov);
        Assert.True(s.Esp.Enabled);
    }
}
