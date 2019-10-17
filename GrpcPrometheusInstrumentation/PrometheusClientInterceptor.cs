using Grpc.Core;
using Grpc.Core.Interceptors;
using Prometheus;

namespace Com.Rfranco.Instrumentation.Prometheus
{
    public class PrometheusClientInterceptor : Interceptor
    {
        private static readonly Counter TotalRequests = Metrics.CreateCounter("requests_total", "Number of processed request.", "method");
        private static readonly Counter TotalResponses = Metrics.CreateCounter("responses_total", "Number of errors processing request.", "method", "error_code");
        private static readonly Summary RequestResponseLatency = Metrics.CreateSummary("requests_duration_summary_seconds", "A Summary of request duration (in seconds) over last 10 minutes.", "method");

        public PrometheusClientInterceptor()
        {
        }
        public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            using (RequestResponseLatency.Labels(method).NewTimer())
            {
                try
                {
                    return  continuation(request, context);
                }
                catch (RpcException e)
                {
                    TotalResponses.Labels(method, e.StatusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    TotalRequests.Labels(method).Inc();
                }
            }
        }
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            using (RequestResponseLatency.Labels(method).NewTimer())
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
                    TotalRequests.Labels(method).Inc();
                }
            }            
        }
        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            using (RequestResponseLatency.Labels(method).NewTimer())
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
                    TotalRequests.Labels(method).Inc();
                }
            }             
        }
        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            using (RequestResponseLatency.Labels(method).NewTimer())
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
                    TotalRequests.Labels(method).Inc();
                }
            }             
        }
        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
        {
            var method = context.Method.FullName.Split('/')[2];
            using (RequestResponseLatency.Labels(method).NewTimer())
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
                    TotalRequests.Labels(method).Inc();
                }
            }             
        }
    }
}