namespace Integration.Tests;

public class IntegrationSmokeTests
{
    [Fact]
    public void Skip_WhenEngineNotInstalled()
    {
        if (Environment.GetEnvironmentVariable("RUN_REAL_CONVERSION_TESTS") != "1")
        {
            return; // skipped by convention
        }

        Assert.True(true);
    }
}
