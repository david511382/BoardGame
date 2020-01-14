using System;
using System.Threading;
using System.Threading.Tasks;
namespace Domain.Api.Services
{
    public static class HandlerService
    {
        public static async Task<bool> Retry(int times, Func<Task<bool>> tryThing, int delayMs = 0)
        {
            if (times == 0)
                return false;

            if (await tryThing())
                return true;

            Thread.Sleep(delayMs);

            return await Retry(times - 1, tryThing);
        }
    }
}
