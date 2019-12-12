namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusInterceptorOptions
    {
        private static readonly double[] DefaultBuckets = {.1, .25, 1, 2.5, 5 };

        public double[] Buckets {get; set;} = DefaultBuckets;
    }
}