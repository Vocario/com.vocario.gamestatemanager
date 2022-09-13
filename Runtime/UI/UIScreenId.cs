using UnityEngine;

public class UIScreenId : ScriptableObject
{
    public UIScreen Screen = null;
    public UIScreen Instance { get; internal set; } = null;
}
