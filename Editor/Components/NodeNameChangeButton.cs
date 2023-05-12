using UnityEngine.UIElements;
using System;

public class NodeNameChangeButton : Button
{
    private bool _isEditing = false;
    public event Action OnEdit;
    public event Action OnSave;

    public NodeNameChangeButton() : base()
    {
        text = "Edit Name";
        clicked += OnClicked;
    }

    private void OnClicked()
    {
        if (_isEditing)
        {
            text = "Edit Name";
            OnSave?.Invoke();
        }
        else
        {
            text = "Save Name";
            OnEdit?.Invoke();
        }
        _isEditing = !_isEditing;
    }
}
