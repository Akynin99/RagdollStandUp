    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MainProvider : MonoBehaviour
    {
        private Dictionary<string, IService> _services;

        private static MainProvider _instance;

        public static MainProvider Instance => _instance;

        private void Awake()
        {
            _instance = this;
            _services = new Dictionary<string, IService>();
        }

        public void RegisterService(IService service, string serviceName)
        {
            _services.Add(serviceName, service);
        }

        public IService Resolve(string serviceName)
        {
            return _services[serviceName];
        }
    }
