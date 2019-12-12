namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusInterceptorOptions
    {
        private static readonly double[] DefaultBuckets = { .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10 };

        public double[] Buckets {get; set;} = DefaultBuckets;
    }
}