    using System;
    using UnityEngine;

    public class StickmanController : MonoBehaviour, IBoneClickObserver
    {
        [SerializeField] private StickmanView View;
        [SerializeField] private float PushForce = 150f;

        private IBoneClickService _boneClickService;
        private IRestartService _restartService;

        private void Start()
        {
            _boneClickService = MainProvider.Instance.Resolve(typeof(IBoneClickService).ToString()) as IBoneClickService;
            if (_boneClickService != null)
                _boneClickService.Subscribe(this);
            
            _restartService = MainProvider.Instance.Resolve(typeof(IRestartService).ToString()) as IRestartService;
            if (_restartService != null)
                _restartService.Subscribe(View);
        }

        private void OnDestroy()
        {
            if(_boneClickService != null)
                _boneClickService.Unsubscribe(this);
        }
        
        public void BoneClicked(Collider bone, Vector3 direction)
        {
            Vector3 force = direction * PushForce;
            View.PushBone(bone, force);
        }
    }
