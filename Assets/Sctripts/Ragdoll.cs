    using System;
    using UnityEngine;

    [Serializable]
    public class Ragdoll
    {
        public Rigidbody[] Rigidbodies;
        public Collider[] Colliders;

        public bool Active;
    }
