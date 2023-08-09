using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartButton : MonoBehaviour
{
    private IRestartService _service;
    
    private void Start()
    {
        _service = MainProvider.Instance.Resolve(typeof(IRestartService).ToString()) as IRestartService;
        GetComponent<Button>().onClick.AddListener(Restart);
    }

    public void Restart()
    {
        _service.Restart();
    }
}
