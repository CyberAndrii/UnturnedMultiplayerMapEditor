using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UnityEngine.LowLevel;
using BF = System.Reflection.BindingFlags;

namespace MultiplayerMapEditor.HostBuilder;

internal sealed class UniTaskPlayerLoopOptions
{
    public int? MainThreadId { get; set; }
}

internal sealed class UniTaskPlayerLoop : IHostedService
{
    private readonly UniTaskPlayerLoopOptions _options;
    private readonly ILogger<UniTaskPlayerLoop> _logger;

    public UniTaskPlayerLoop(
        IOptions<UniTaskPlayerLoopOptions> options,
        ILogger<UniTaskPlayerLoop> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        EnsureInjected(
            _options.MainThreadId ?? throw new InvalidOperationException("Main thread id is not set"),
            exception => _logger?.LogError(exception, "Caught UnobservedTaskException in UniTask's PlayerLoop")
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static void EnsureInjected(int mainThreadId, Action<Exception> onUnobservedTaskException)
    {
        if (!PlayerLoopHelper.IsInjectedUniTaskPlayerLoop())
        {
            var unitySynchronizationContextField =
                typeof(PlayerLoopHelper).GetField("unitySynchronizationContext", BF.Static | BF.NonPublic);

            // For older version of UniTask
            unitySynchronizationContextField ??=
                typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext", BF.Static | BF.NonPublic)
                ?? throw new Exception("Could not find PlayerLoopHelper.unitySynchronizationContext field");

            unitySynchronizationContextField.SetValue(null, SynchronizationContext.Current);

            var mainThreadIdField =
                typeof(PlayerLoopHelper).GetField("mainThreadId", BF.Static | BF.NonPublic)
                ?? throw new Exception("Could not find PlayerLoopHelper.mainThreadId field");

            mainThreadIdField.SetValue(null, mainThreadId);

            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            PlayerLoopHelper.Initialize(ref playerLoop);

            // Handle UniTask exception
            UniTaskScheduler.UnobservedTaskException += onUnobservedTaskException;
        }

        // Do not switch thread
        UniTaskScheduler.DispatchUnityMainThread = false;
    }
}
