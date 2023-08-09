    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class BoneClickService : MonoBehaviour, IBoneClickService
    {
        [SerializeField] private LayerMask BoneLayerMask;
        private bool _mouseDownLastFrame;

        private List<IBoneClickObserver> _observers;

        private void Awake()
        {
            _observers = new List<IBoneClickObserver>();
            MainProvider.Instance.RegisterService(this, typeof(IBoneClickService).ToString());
        }

        private void Update()
        {
            bool mouseDown = Input.GetMouseButton(0);

            if (!mouseDown && _mouseDownLastFrame)
            {
                Click();
            }

            _mouseDownLastFrame = mouseDown;
        }

        private void Click()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(!Physics.Raycast(ray, out RaycastHit hit, 100, BoneLayerMask))
               return;

            var view = hit.collider.GetComponentInParent<StickmanView>();

            if (view != null)
            {
                BoneClicked(hit.collider, ray.direction);
            }
            else
            {
                Debug.LogError("Can't find bone!");
            }
        }

        private void BoneClicked(Collider bone, Vector3 direction)
        {
            foreach (var observer in _observers)
            {
                observer.BoneClicked(bone, direction);
            }
        }
        
        public void Subscribe(IBoneClickObserver obs)
        {
            if(!_observers.Contains(obs))
                _observers.Add(obs);
        }

        public void Unsubscribe(IBoneClickObserver obs)
        {
            if(_observers.Contains(obs))
                _observers.Remove(obs);
        }


        
    }
