namespace Kestrel.Socket.Server;

public sealed class ShowThreadCountHostServer : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(2000, stoppingToken);
            
            ThreadPool.GetMinThreads(out var minWorkingThreads, out var minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out var maxWorkingThreads, out var maxCompletionPortThreads);

            Console.WriteLine($"最小工作线程数：{minWorkingThreads}-最小完成端口数：{minCompletionPortThreads}");
            Console.WriteLine($"最大工作线程数：{maxWorkingThreads}-最大完成端口数：{maxCompletionPortThreads}");

        }
    }
}