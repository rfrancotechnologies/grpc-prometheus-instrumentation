using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Prometheus;

namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusServerInterceptor : Interceptor
    {
        private Counter ErrorRequestsProcessed;
        private Gauge OngoingRequests;
        private Histogram RequestResponseHistogram;

        public PrometheusServerInterceptor(string prefix = "server")
        {
            ErrorRequestsProcessed = Metrics.CreateCounter($"{prefix}_grpc_error_total", "Number of errors processing request.", "method", "error_code");
            OngoingRequests = Metrics.CreateGauge($"{prefix}_grpc_requests_in_progress", "Number of ongoing requests.", "method");
            RequestResponseHistogram = Metrics.CreateHistogram($"{prefix}_grpc_requests_duration_histogram_seconds", "Histogram of request duration in seconds.", "method");
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
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
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
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
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
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
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var method = context.Method.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
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
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }
    }
}