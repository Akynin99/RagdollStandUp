    using System;
    using System.Linq;
    using UnityEngine;

    public class StickmanView : MonoBehaviour, IRestartable
    {
        [SerializeField] private Ragdoll Ragdoll;
        [SerializeField] private CapsuleCollider MainCollider;
        [SerializeField] private Rigidbody MainRigidbody;
        [SerializeField] private bool SpawnAsRagdoll;
        [SerializeField] private Animator Animator;
        [SerializeField] private float MaxBoneSpeedForStanding;
        [SerializeField] private float MinRagdollTime;
        [SerializeField] private float MaxRagdollTime;
        [SerializeField] private float LerpBonesTime;
        [SerializeField] private string StandUpAnim;

        private Vector3 _startPos;
        private Quaternion _startRot;
        private float _ragdollTime;
        private float _lerpBonesTimer;
        private Transform _hipsBone;
        private int _standUpAnimHash;
        private StickmanState _currentState;
        private BoneTransform[] _standUpBoneTransforms;
        private BoneTransform[] _ragdollBoneTransforms;
        private Transform[] _bones;

        private void Awake()
        {
            _startPos = transform.position;
            _startRot = transform.rotation;
            _hipsBone = Animator.GetBoneTransform(HumanBodyBones.Hips);
            _standUpAnimHash = Animator.StringToHash(StandUpAnim);
            _bones = _hipsBone.GetComponentsInChildren<Transform>();
            _standUpBoneTransforms = new BoneTransform[_bones.Length];
            _ragdollBoneTransforms = new BoneTransform[_bones.Length];
            for (int i = 0; i < _bones.Length; i++)
            {
                _standUpBoneTransforms[i] = new BoneTransform();
                _ragdollBoneTransforms[i] = new BoneTransform();
            }

            SaveAnimationStartBoneTransforms(StandUpAnim, _standUpBoneTransforms);
            RagdollSetActive(SpawnAsRagdoll);
        }

        private void Start()
        {
            
        }

        private void RagdollSetActive(bool value)
        {
            foreach (var rigidbody in Ragdoll.Rigidbodies)
            {
                rigidbody.isKinematic = !value;
            }

            // MainCollider.enabled = !value;
            Animator.enabled = !value;
            Ragdoll.Active = value;

            if (value)
                _ragdollTime = Time.time;

            _currentState = value ? StickmanState.Ragdoll : StickmanState.Idle;
        }

        private void Update()
        {
            switch (_currentState)
            {
                case StickmanState.Idle:
                    IdleBehaviour();
                    break;
                
                case StickmanState.Ragdoll:
                    RagdollBehaviour();
                    break;
                
                case StickmanState.StandingUp:
                    StandingUpBehaviour();
                    break;
                
                case StickmanState.LerpBones:
                    LerpBonesBehaviour();
                    break;
            }
        }

        private void IdleBehaviour()
        {
        }
        
        private void RagdollBehaviour()
        {
            float maxBoneSpeed = 0;

            foreach (var rigidbody in Ragdoll.Rigidbodies)
            {
                if (rigidbody.velocity.magnitude > maxBoneSpeed)
                    maxBoneSpeed = rigidbody.velocity.magnitude;
            }
                
            if(maxBoneSpeed < MaxBoneSpeedForStanding
               && Time.time > _ragdollTime + MinRagdollTime
               || Time.time > _ragdollTime + MaxRagdollTime)
                StartLerpBones();
        }
        
        private void StandingUpBehaviour()
        {
            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(StandUpAnim) == false)
            {
                // standing up animation is finished
                _currentState = StickmanState.Idle;
            }
        }

        private void LerpBonesBehaviour()
        {
            _lerpBonesTimer += Time.deltaTime;
            float lerpProgress = _lerpBonesTimer / LerpBonesTime;

            for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex ++)
            {
                _bones[boneIndex].localPosition = Vector3.Lerp(
                    _ragdollBoneTransforms[boneIndex].Position,
                    _standUpBoneTransforms[boneIndex].Position,
                    lerpProgress);

                _bones[boneIndex].localRotation = Quaternion.Lerp(
                    _ragdollBoneTransforms[boneIndex].Rotation,
                    _standUpBoneTransforms[boneIndex].Rotation,
                    lerpProgress);
            }

            if (lerpProgress >=1)
            {
                StandUp();

            }
        }
        
        private void SaveAnimationStartBoneTransforms(string clipName, BoneTransform[] boneTransforms)
        {
            Vector3 positionBeforeSampling = transform.position;
            Quaternion rotationBeforeSampling = transform.rotation;
            foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == clipName)
                {
                    clip.SampleAnimation(Animator.gameObject, 0f);
                    SaveBoneTransforms(_standUpBoneTransforms);
                    break;
                }
            }
            transform.position = positionBeforeSampling;
            transform.rotation = rotationBeforeSampling;
        }
        
        private void SaveBoneTransforms(BoneTransform[] boneTransforms)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                boneTransforms[i].Position = _bones[i].localPosition;
                boneTransforms[i].Rotation = _bones[i].localRotation;
            }
        }

        private void StartLerpBones()
        {
            
            AlignPositionToHips();
            AlignRotationToHips();
            SaveBoneTransforms(_ragdollBoneTransforms);
            
            _lerpBonesTimer = 0;
            _currentState = StickmanState.LerpBones;
        }

        private void StandUp()
        {
            RagdollSetActive(false);
            
            
            Animator.Play(_standUpAnimHash, 0, 0);
            
            _currentState = StickmanState.StandingUp;
        }

        public void PushBone(Collider bone, Vector3 force)
        {
            if (bone == null || !Ragdoll.Colliders.Contains(bone))
            {
                Debug.LogError("Wrong bone!");
                return;
            }
            
            RagdollSetActive(true);
            
            bone.attachedRigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        public void Restart()
        {
            RagdollSetActive(SpawnAsRagdoll);
            transform.position = _startPos;
            transform.rotation = _startRot;
            Animator.Rebind();
            Animator.Update(0f);

        }
        
        private void AlignPositionToHips()
        {
            Vector3 originalHipsPosition = _hipsBone.position; 
            transform.position = _hipsBone.position;

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo))
            {
                transform.position = new Vector3(transform.position.x, hitInfo.point.y, transform.position.z);
            }
        
            _hipsBone.position = originalHipsPosition;
        }
        
        private void AlignRotationToHips()
        {
            Vector3 originalHipsPosition = _hipsBone.position;
            Quaternion originalHipsRotation = _hipsBone.rotation;
        
            Vector3 desiredDirection = _hipsBone.up * -1;
            desiredDirection.y = 0;
            desiredDirection.Normalize();
        
            Quaternion fromToRotation = Quaternion.FromToRotation(transform.forward, desiredDirection);
            transform.rotation *= fromToRotation;
        
            _hipsBone.position = originalHipsPosition;
            _hipsBone.rotation = originalHipsRotation;
        }
        
        private enum StickmanState
        {
            Idle = 0,
            Ragdoll = 1,
            StandingUp = 2,
            LerpBones = 3,
        }
        
        private class BoneTransform
        {
            public Vector3 Position { get; set; }

            public Quaternion Rotation { get; set; }
        }
    }
