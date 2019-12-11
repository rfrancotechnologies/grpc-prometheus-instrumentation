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

        public PrometheusServerInterceptor()
        {
            ErrorRequestsProcessed = Metrics.CreateCounter($"server_request_error_total", "Number of errors processing messages.", "service","method", "error_code");
            OngoingRequests = Metrics.CreateGauge($"server_request_in_progress", "Number of ongoing messages.", "service", "method");
            RequestResponseHistogram = Metrics.CreateHistogram($"server_request_duration_seconds", "Histogram of message duration in seconds.", "service", "method");
        }
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var info = context.Method.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return await continuation(request, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }

        public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var info = context.Method.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(requestStream, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }

        public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var info = context.Method.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(request, responseStream, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }

        public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            var info = context.Method.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(requestStream, responseStream, context);
                }
                catch (RpcException e)
                {
                    ErrorRequestsProcessed.Labels(service , method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service , method).Dec();
                }
            }
        }
    }
}