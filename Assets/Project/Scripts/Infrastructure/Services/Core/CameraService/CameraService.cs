using System;
using DG.Tweening;
using Infrastructure.Services.Gameplay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Infrastructure.Services.Core
{
    public class CameraService : IService
    {
        private IHexGridService _hexGridService;

        public CameraGroup CameraGroup { get; private set; }

        public Camera Camera { get; private set; }

        public void Initialize(IHexGridService hexGridService)
        {
            _hexGridService = hexGridService;
            CameraGroup = Object.FindObjectOfType<CameraGroup>();
            Camera = Camera.main;
            //SetWidth(10f);
        }

        public void SetFollow(GameObject follow)
        {
            CameraGroup.VirtualCamera.Follow = follow.transform;
            CameraGroup.VirtualCamera.LookAt = follow.transform;
        }

        // For portrait mobile
        public void SetWidth(float width)
        {
            Camera.ResetAspect();
            var _1OverAspect = 1f / Camera.aspect;
            var finalValue = width * .5f * _1OverAspect;
            CameraGroup.VirtualCamera.Lens.OrthographicSize = finalValue;
        }

        public void NextStage(Action stageMoved)
        {
            CameraGroup.transform.DOMoveY(-40f, 2f).SetRelative().OnComplete(stageMoved.Invoke);
        }

        public void SetCenterPos()
        {
            var gridCenter = _hexGridService.GetGridCenterPos();
            var delta = gridCenter - CameraGroup.TargetPoint.position;

            CameraGroup.TargetPoint.position = gridCenter;
            CameraGroup.VirtualCamera.OnTargetObjectWarped(CameraGroup.TargetPoint, delta);
        }
    }
}