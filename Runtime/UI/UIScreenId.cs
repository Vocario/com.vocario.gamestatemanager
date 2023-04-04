using UnityEngine;

public class UIScreenId : ScriptableObject
{
    public AUIScreen Screen = null;
    public AUIScreen Instance { get; internal set; } = null;
}
