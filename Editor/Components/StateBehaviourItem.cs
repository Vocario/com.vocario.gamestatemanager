using UnityEngine.UIElements;
using System;
using UnityEngine;

public class StateBehaviourItem : VisualElement
{
    private Func<Guid, string, bool> _removeItemCallback;
    private Guid _nodeId;
    private string _itemId;

    public StateBehaviourItem(Guid nodeId, string itemId, Func<Guid, string, bool> removeItemCallback) : base()
    {
        _nodeId = nodeId;
        _itemId = itemId;
        focusable = true;
        pickingMode = PickingMode.Position;
        _removeItemCallback = removeItemCallback;
        RegisterCallback((ContextualMenuPopulateEvent evt) => BuildContextualMenu(evt));
    }

    private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        Vector2 nodePosition = this.ChangeCoordinatesTo(contentContainer, evt.localMousePosition);

        evt.menu.AppendAction("Remove Behaviour", a => _removeItemCallback?.Invoke(_nodeId, _itemId));
    }
}
