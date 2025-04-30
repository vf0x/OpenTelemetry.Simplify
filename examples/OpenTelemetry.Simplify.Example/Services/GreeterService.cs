using OpenTelemetry.Simplify.Example.Sample;
using Grpc.Core;
using Oracle.ManagedDataAccess.Client;
using System.Net.Http;

namespace OpenTelemetry.Simplify.Example.Sample.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public GreeterService(ILogger<GreeterService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            _logger.LogInformation("SayHello called with {Name}", request.Name);

            return new HelloReply
            {
                Message = "Hello " + request.Name
            };
        }

        public override async Task<HelloReply> HttpCall(HelloRequest request, ServerCallContext context)
        {
            var client = _httpClientFactory.CreateClient("httpbin");
            var response = await client.GetAsync("get");
            var content = await response.Content.ReadAsStringAsync();

            return new HelloReply
            {
                Message = $"HTTP status: {response.StatusCode}"
            };
        }

        public override async Task<HelloReply> DbQuery(HelloRequest request, ServerCallContext context)
        {
            try
            {
                using (var connection = new OracleConnection(""))
                {
                    await connection.OpenAsync();
                    using var cmd = connection.CreateCommand();
                    cmd.CommandText = "SELECT 1 FROM DUAL";
                    var result = await cmd.ExecuteScalarAsync();
                    return new HelloReply
                    {
                        Message = $"DB result: {result}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new HelloReply
                {
                    Message = $"DB error: {ex.Message}"
                };
            }
        }

        public override Task<HelloReply> CpuStress(HelloRequest request, ServerCallContext context)
        {
            var sum = 0L;
            for (int i = 0; i < 100_000_000; i++)
                sum += i;

            return Task.FromResult(new HelloReply
            {
                Message = $"CPU stress result: {sum}"
            });
        }

        public override Task<HelloReply> MemoryStress(HelloRequest request, ServerCallContext context)
        {
            var bigList = new List<byte[]>();
            for (int i = 0; i < 100; i++)
            {
                bigList.Add(new byte[10_000_000]); 
                Thread.Sleep(10); 
            }

            return Task.FromResult(new HelloReply
            {
                Message = "Memory stress allocated"
            });
        }

        public override Task<HelloReply> RandomException(HelloRequest request, ServerCallContext context)
        {
            try
            {
                var exceptions = new List<Exception>
            {
                new InvalidOperationException(),
                new ArgumentNullException(),
                new ArgumentOutOfRangeException(),
                new NotImplementedException(),
                new TimeoutException(),
                new HttpRequestException(),
                new NullReferenceException(),
                new DivideByZeroException(),
                new FormatException(),
                new UnauthorizedAccessException()
            };

                var random = new Random();
                int index = random.Next(exceptions.Count);
                throw exceptions[index];
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
        }
    }
}
