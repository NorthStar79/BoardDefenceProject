using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public sealed class LevelButtonView : MonoBehaviour
{
    public TMP_Text title;
    public Image thumbnail;

    private Action _onClick;

    private void Awake()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(HandleClick);
    }

    public void Bind(string displayName, Sprite sprite, Action onClick)
    {
        if (title != null) title.text = displayName;
        if (thumbnail != null) thumbnail.sprite = sprite;
        _onClick = onClick;
    }

    private void HandleClick() => _onClick?.Invoke();
}
