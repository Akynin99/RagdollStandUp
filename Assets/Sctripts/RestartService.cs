using System.Collections.Generic;
using UnityEngine;

public class RestartService : MonoBehaviour, IRestartService
{
    private List<IRestartable> _restartables = new List<IRestartable>();

    private void Awake()
    {
        MainProvider.Instance.RegisterService(this, typeof(IRestartService).ToString());
    }

    public void Restart()
    {
        foreach (var restartable in _restartables)
        {
            restartable.Restart();
        }
    }

    public void Subscribe(IRestartable restartable)
    {
        if (!_restartables.Contains(restartable))
            _restartables.Add(restartable);
    }

    public void Unsubscribe(IRestartable restartable)
    {
        if (_restartables.Contains(restartable))
            _restartables.Remove(restartable);
    }
}
