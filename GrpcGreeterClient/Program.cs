using System;
using System.Net.Http;
using System.Threading.Tasks;

using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress("https://localhost:5000",
                new GrpcChannelOptions { HttpHandler = httpHandler });
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
