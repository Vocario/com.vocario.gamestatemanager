using System;

using UnityEngine;
using UnityEngine.UIElements;

namespace Vocario.UI
{
    //TODO Create an editor create wizard
    [Serializable]
    public abstract class AController : ScriptableObject
    {
        [SerializeField]
        protected VisualTreeAsset _screenTemplate = null;
        // TODO Might want to cache this element at one point instead of always cloning
        protected int _currentElementIndex = -1;
        protected UIDocument _currentDocument = null;
        protected bool _isActive = false;

        internal void Hide()
        {
            if (_currentDocument == null)
            {
                Debug.LogWarning($"[UI @ {GetType()}] - Attempted to hide screen {_screenTemplate} when screen already hidden");
                return;
            }
            _currentDocument.rootVisualElement.RemoveAt(_currentElementIndex);
            _currentDocument = null;
            _isActive = false;
        }

        internal void Show(UIDocument mainUIDocument)
        {
            if (_currentDocument != null)
            {
                Debug.LogWarning($"[UI @ {GetType()}] - Attempted to show screen {_screenTemplate} when screen already showned");
                return;
            }
            _currentDocument = mainUIDocument;
            _isActive = true;
            _screenTemplate.CloneTree(_currentDocument.rootVisualElement, out _currentElementIndex, out int _);
        }
    }
}
