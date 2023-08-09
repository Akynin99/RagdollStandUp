    using System;
    using UnityEngine;

    public class Cube : MonoBehaviour, IRestartable
    {
        [SerializeField] private Rigidbody Rigidbody;
        
        private IRestartService _restartService;
        private Vector3 _startPos;
        private Quaternion _startRot;

        private void Start()
        {
            _startPos = transform.position;
            _startRot = transform.rotation;
            _restartService = MainProvider.Instance.Resolve(typeof(IRestartService).ToString()) as IRestartService;
            if (_restartService != null)
                _restartService.Subscribe(this);
        }

        public void Restart()
        {
            transform.position = _startPos;
            transform.rotation = _startRot;
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }
    }
