using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Microsoft.Extensions.Hosting;

namespace MultiplayerMapEditor.Editor.Level.Objects.UpdateProperties;

internal sealed class EditorLevelObjectsUI : IHostedService, IDisposable
{
    private readonly INetObjectPropertiesUpdater _netObjectPropertiesUpdater;
    private ISleekField? _materialPaletteOverrideField;
    private ISleekInt32Field? _materialIndexOverrideField;
    private readonly CancellationTokenSource _cts = new();

    public EditorLevelObjectsUI(INetObjectPropertiesUpdater netObjectPropertiesUpdater)
    {
        _netObjectPropertiesUpdater = netObjectPropertiesUpdater;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        SubscribeAsync().Forget();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Unsubscribe();
        _cts.Cancel();
        return Task.CompletedTask;
    }

    private async UniTaskVoid SubscribeAsync()
    {
        var token = _cts.Token;
        await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate().WithCancellation(token))
        {
            await UniTask.WaitUntil(() =>
            {
                _materialPaletteOverrideField = EditorLevelObjectsUIReflection.GetMaterialPaletteOverrideField();
                _materialIndexOverrideField = EditorLevelObjectsUIReflection.GetMaterialIndexOverrideField();
                return _materialPaletteOverrideField != null && _materialIndexOverrideField != null;
            }, cancellationToken: token);

            _materialPaletteOverrideField!.onTyped += OnTypedMaterialPaletteOverride;
            _materialIndexOverrideField!.onTypedInt += OnTypedMaterialIndexOverride;

            break;
        }
    }

    private void Unsubscribe()
    {
        if (_materialPaletteOverrideField != null)
        {
            _materialPaletteOverrideField.onTyped -= OnTypedMaterialPaletteOverride;
        }

        if (_materialIndexOverrideField != null)
        {
            _materialIndexOverrideField.onTypedInt -= OnTypedMaterialIndexOverride;
        }
    }

    private void OnTypedMaterialPaletteOverride(ISleekField field, string text)
    {
        Send();
    }

    private void OnTypedMaterialIndexOverride(ISleekInt32Field field, int value)
    {
        Send();
    }

    private void Send()
    {
        var focusedObject = EditorLevelObjectsUIReflection.GetFocusedLevelObject();

        if (focusedObject == null ||
            !NetIdRegistry.GetTransformNetId(focusedObject.transform, out var netId, out var path))
        {
            return;
        }

        _netObjectPropertiesUpdater.UpdateObjectProperties(
            netId,
            new AssetReference<MaterialPaletteAsset>(_materialPaletteOverrideField!.text).GUID,
            _materialIndexOverrideField!.state
        );
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}
