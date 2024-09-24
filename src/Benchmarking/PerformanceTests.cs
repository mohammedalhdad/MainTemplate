using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;


[MemoryDiagnoser]
[Config(typeof(Config))]
public class PerformanceTests
{
    private class Config : ManualConfig
    {
        public Config()
        {
            SummaryStyle = SummaryStyle.Default.WithRatioStyle(RatioStyle.Trend);
        }
    }


    //private readonly MyService _myService;

    public PerformanceTests()
    {
        // فرضاً أنك تقوم باختبار خدمة من الطبقة Application
        //_myService = new MyService();
    }

    [Benchmark]
    public void TestMethodPerformance()
    {
        // فرضاً أنك تختبر أداء دالة معينة
        //_myService.DoWork();
    } 
    
    [Benchmark(Baseline = true)]

    public void TestMethodPerformance2()
    {
        // فرضاً أنك تختبر أداء دالة معينة
        //_myService.DoWork();
    }
}
