using UnityEngine;

[DisallowMultipleComponent]
public sealed class GhostTint : MonoBehaviour
{
    public Material ghostMaterialOverride;

    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;
    private int _colorPropId = -1;

    private bool _ghostActive;
    private Material[][] _originalMats;
    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        _mpb = new MaterialPropertyBlock();

        // URP/HDRP: _BaseColor, legacy: _Color
        foreach (var r in _renderers)
        {
            if (r == null || r.sharedMaterial == null) continue;
            if (r.sharedMaterial.HasProperty("_BaseColor")) { _colorPropId = Shader.PropertyToID("_BaseColor"); break; }
            if (r.sharedMaterial.HasProperty("_Color"))     { _colorPropId = Shader.PropertyToID("_Color");     break; }
        }
    }

    public void SetTint(Color c)
    {
        if (_renderers == null) return;

        _mpb.Clear();
        _mpb.SetColor(_colorPropId, c);

        for (int i = 0; i < _renderers.Length; i++)
        {
            var r = _renderers[i];
            if (r == null) continue;
            r.SetPropertyBlock(_mpb);
        }
    }

    public void SetGhostMode(bool enabled)
    {
        if (_ghostActive == enabled) return;
        _ghostActive = enabled;

        if (ghostMaterialOverride == null || _renderers == null) return;

        if (enabled)
        {
            _originalMats = new Material[_renderers.Length][];
            for (int i = 0; i < _renderers.Length; i++)
            {
                var r = _renderers[i];
                if (r == null) continue;

                var mats = r.sharedMaterials;
                _originalMats[i] = mats;

                var ghostArray = new Material[mats.Length];
                for (int m = 0; m < mats.Length; m++) ghostArray[m] = ghostMaterialOverride;
                r.sharedMaterials = ghostArray;
            }
        }
        else
        {
            // Revert originals
            if (_originalMats != null)
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    var r = _renderers[i];
                    if (r == null) continue;

                    var mats = (i < _originalMats.Length) ? _originalMats[i] : null;
                    if (mats != null) r.sharedMaterials = mats;
                }
            }
            _originalMats = null;
        }
    }
}
