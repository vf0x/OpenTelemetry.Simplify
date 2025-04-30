using OpenTelemetry.Simplify.Example.Sample.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace OpenTelemetry.Simplify.Example.Sample.Services
{
    public class RandomService : IRandomService
    {
        public RandomService() { }

        public int GetRandomNumber()
        {
            Random rnd = new Random(); 
            int month = rnd.Next(1000);
            return month;
        }
    }
}
