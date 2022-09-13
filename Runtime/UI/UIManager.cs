using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UIManager_", menuName = "Scriptable/UIManager", order = 11)]
public class UIManager : ScriptableObject
{
    [SerializeField]
    private List<UIScreenId> _screenIds = new List<UIScreenId>();
    public string ScreenPrefabsPath;

    public void Show(UIScreenId id) => UIManagerBehaviour.Instance.Show(id);
    public void Hide(UIScreenId id) => UIManagerBehaviour.Instance.Hide(id);

    public void AddScreenId(UIScreenId screenId) => _screenIds.Add(screenId);
}
