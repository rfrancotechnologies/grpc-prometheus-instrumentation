# Com.RFranco.Instrumentation.Prometheus

A very simple library to instrumentalize your gRPC client / services with Prometheus using interceptors.
For more information : https://github.com/grpc/proposal/blob/master/L12-csharp-interceptors.md).

## Usage

An example of use of this type of this library on the server side would be:

```csharp
    Server server = new Server
    {
        Services = { Service.BindService(new ServiceImpl()).Intercept(new PrometheusServerInterceptor())},
        Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
    };

```

An example of use of this type of this library on the client side would be:

```csharp
    var client = new ServerClient(channel.Intercept(new PrometheusClientInterceptor()));
```
