using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class HudBase : MonoBehaviour
{
    public TMP_Text baseLabel;

    private SignalHub hub;
    private void Awake() { hub = FindFirstObjectByType<SignalHub>(); }
    private void OnEnable()
    {
        if (hub == null) return;
        hub.BaseDamaged += OnBaseDamaged;
    }
    private void OnDisable()
    {
        if (hub == null) return;
        hub.BaseDamaged -= OnBaseDamaged;
    }
    private void OnBaseDamaged(int hp, int max, int delta)
    {
        if (baseLabel != null) baseLabel.text = $"Base: {hp}/{max}";
    }
}
