using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Prometheus;

namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusServerInterceptor : Interceptor
    {
        private static readonly Counter RequestsProcessed = Metrics.CreateCounter("requests_processed_total", "Number of processed request.", "method");
        private static readonly Counter ErrorRequestsProcessed = Metrics.CreateCounter("requests_error_total", "Number of errors processing request.", "method", "error_code");
        private static readonly Summary RequestDurationSummaryInSeconds = Metrics.CreateSummary("requests_duration_summary_seconds", "A Summary of request duration (in seconds) over last 10 minutes.", "method");

        public PrometheusServerInterceptor()
        {
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            using (RequestDurationSummaryInSeconds.Labels(method).NewTimer())
            {
                try
                {
                    return await continuation(request, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    RequestsProcessed.Labels(method).Inc();
                }
            }

        }


        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            using (RequestDurationSummaryInSeconds.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(requestStream, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    RequestsProcessed.Labels(method).Inc();
                }
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            using (RequestDurationSummaryInSeconds.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(request, responseStream, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    RequestsProcessed.Labels(method).Inc();
                }
            }
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            using (RequestDurationSummaryInSeconds.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(requestStream, responseStream, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    RequestsProcessed.Labels(method).Inc();
                }
            }
        }
    }
}