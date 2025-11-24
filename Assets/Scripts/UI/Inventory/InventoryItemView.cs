using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using TMPro;

[DisallowMultipleComponent]
public sealed class InventoryItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public TMP_Text label;
    public TMP_Text countLabel;
    public Image icon;

    public DefenseItemType Type { get; private set; }
    public Action<DefenseItemType, Vector2> onBeginDragItem;

    public void Bind(DefenseItemType type, int count)
    {
        Type = type;
        if (label != null) label.text = type != null ? type.displayName : "Unknown";
        SetCount(count);
    }

    public void SetCount(int count)
    {
        if (countLabel != null) countLabel.text = count.ToString();
        if (icon != null) icon.color = (count > 0) ? Color.white : new Color(1f,1f,1f,0.38f);
        var btn = GetComponent<Button>();
        if (btn != null) btn.interactable = count > 0;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Type == null) return;
        onBeginDragItem?.Invoke(Type, eventData.position);
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
    }
}
