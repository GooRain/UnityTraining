using System;
using Extensions;
using Service;
using Service.Updating;
using Utility;

namespace FantasyGame.GamePlay
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class SmoothOrbitLook : MonoBehaviour, IUpdateable
    {
        [SerializeField]
        private float lookSensitivity;

        [SerializeField]
        private Transform target;

        [SerializeField]
        private float distanceFromTarget;

        [SerializeField, Range(-360f, 360f)]
        private float pitchMin;

        [SerializeField, Range(-360f, 360f)]
        private float pitchMax;

        [SerializeField]
        private float rotationSmoothTime;

        [SerializeField]
        private Vector3 startRotationAngle;

        [SerializeField]
        private SharedQuaternion sharedDirection;

        private float yaw;
        private float pitch;

        private Transform cachedTransform;
        private Vector3 currentRotation;
        private Vector3 rotationSmoothVelocity;
        private Quaternion rotation;

        public bool IsEnabled { get; }

        private void Awake()
        {
            cachedTransform = transform;

            pitch = startRotationAngle.x;
            yaw = startRotationAngle.y;
            currentRotation = startRotationAngle;

            ComponentLocator.Resolve<Updater>().Register(UpdateType.Default, this);
            ComponentLocator.Resolve<Updater>().Register(UpdateType.Late, this);
        }

        public void DoUpdate(float deltaTime)
        {
            InputBehaviour();
        }

        public void DoLateUpdate(float deltaTime)
        {
            SmoothLook();
        }

        private void InputBehaviour()
        {
            yaw += Input.GetAxis(StringExt.MouseX) * lookSensitivity;
            pitch -= Input.GetAxis(StringExt.MouseY) * lookSensitivity;
        }

        private void SmoothLook()
        {
            rotation = cachedTransform.rotation;

            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity,
                rotationSmoothTime);

            rotation.eulerAngles = currentRotation;
            cachedTransform.position = target.position - cachedTransform.forward * distanceFromTarget;
            sharedDirection.SetValue(rotation);

            cachedTransform.rotation = rotation;
        }

        public void DoFixedUpdate(float fixedDeltaTime)
        {
            throw new NotImplementedException();
        }

        private void OnDestroy()
        {
            ComponentLocator.Resolve<Updater>().Delete(UpdateType.Default, this);
            ComponentLocator.Resolve<Updater>().Delete(UpdateType.Late, this);
        }
    }
}