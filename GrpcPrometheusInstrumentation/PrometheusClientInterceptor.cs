using Grpc.Core;
using Grpc.Core.Interceptors;
using Prometheus;

namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusClientInterceptor : Interceptor
    {
        private Counter TotalResponses;
        private Gauge OngoingRequests;
        private Histogram RequestResponseHistogram;


        public PrometheusClientInterceptor(string prefix = "client")
        {
            TotalResponses = Metrics.CreateCounter($"{prefix}_grpc_error_total", "Number of errors processing request.", "method", "error_code");
            OngoingRequests = Metrics.CreateGauge($"{prefix}_grpc_requests_in_progress", "Number of ongoing requests.", "method");
            RequestResponseHistogram = Metrics.CreateHistogram($"{prefix}_grpc_requests_duration_histogram_seconds", "Histogram of request duration in seconds.", "method");
        }
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalResponses.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalResponses.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalResponses.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(context);
                }
                catch (RpcException e)
                {
                    TotalResponses.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(method).Dec();
                }
            }
        }
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            OngoingRequests.Labels(method).Inc();

            using (RequestResponseHistogram.Labels(method).NewTimer())
            {
                try
                {
                    return continuation(context);
                }
                catch (RpcException e)
                {
                    TotalResponses.Labels(method, e.StatusCode.ToString()).Inc();
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