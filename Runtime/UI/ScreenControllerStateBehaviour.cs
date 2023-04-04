using Vocario.GameStateManager;
using System;
using Vocario.UI;
using UnityEngine;

public class ScreenControllerStateBehaviour : AStateBehaviour
{
    [SerializeField]
    private AController _uIController = null;

    public override void OnEnter() => UIManagerBehaviour.Instance.ShowScreen(_uIController);

    public override void OnExit() => Debug.Log($"Exiting State");
}

