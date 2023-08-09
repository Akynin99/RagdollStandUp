    using UnityEngine;

    public interface IBoneClickObserver
    {
        public void BoneClicked(Collider bone, Vector3 direction);
    }
