using Grpc.Core;
using Grpc.Core.Interceptors;
using Prometheus;

namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusClientInterceptor : Interceptor
    {
        private Counter TotalErrors;
        private Gauge OngoingRequests;
        private Histogram RequestResponseHistogram;

        private string ServiceName;


        public PrometheusClientInterceptor()
        {
            TotalErrors = Metrics.CreateCounter($"client_request_error_total", "Number of errors processing messages.", "service", "method", "error_code");
            OngoingRequests = Metrics.CreateGauge($"client_request_in_progress", "Number of ongoing messages.", "service", "method");
            RequestResponseHistogram = Metrics.CreateHistogram($"client_request_duration_seconds", "Histogram of messages duration in seconds.", "service", "method");
        }
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var info = context.Method.FullName.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalErrors.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var info = context.Method.FullName.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalErrors.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var info = context.Method.FullName.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalErrors.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var info = context.Method.FullName.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(context);
                }
                catch (RpcException e)
                {
                    TotalErrors.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service, method).Dec();
                }
            }
        }
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var info = context.Method.FullName.Split('/');
            var service = info[1];
            var method = info[2];
            OngoingRequests.Labels(service, method).Inc();

            using (RequestResponseHistogram.Labels(service, method).NewTimer())
            {
                try
                {
                    return continuation(context);
                }
                catch (RpcException e)
                {
                    TotalErrors.Labels(service, method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    OngoingRequests.Labels(service,method).Dec();
                }
            }
        }
    }
}