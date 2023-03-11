using UnityEngine.UIElements;
using UnityEngine;

namespace Vocario.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManagerBehaviour : SingletonMonoBehaviour<UIManagerBehaviour>
    {
        protected AController _activeScreenController = null;
        protected UIDocument _mainUIDocument = null;

        internal UIDocument MainUIDocument
        {
            get
            {
                if (_mainUIDocument == null)
                {
                    _mainUIDocument = GetComponent<UIDocument>();
                }
                return _mainUIDocument;
            }
        }

        public bool ShowScreen(AController controller)
        {
            if (_activeScreenController == controller)
            {
                return false;
            }
            _activeScreenController.Hide();
            _activeScreenController = controller;
            _activeScreenController.Show(MainUIDocument);
            return true;
        }
    }
}
