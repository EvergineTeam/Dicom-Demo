using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;
using System;
using Evergine.Common.Attributes;
using System.Diagnostics;

namespace DicomDemo.OrbitCamera
{
    public abstract class CameraZoomBehavior : InputObserver
    {
        private const float SmoothTimeMilliseconds = 100f;

        private float smoothDampDeriv;

        public float ZoomFactor = 8.0f;
        public float MinDistance = 50.0f;
        public float MaxDistance = 250.0f;
        public bool InverseScroll;

        [IgnoreEvergine] 
        public float DesiredDistance;

        public float InitialDistance;

        private Transform3D cameraTransform;

        protected abstract bool TryGetScrollPosition(out float scroll);

        protected virtual bool IsZoomRequested() => false;

        protected override void Start()
        {
            base.Start();

            this.cameraTransform = this.Camera.Owner.FindComponent<Transform3D>();

            // intialize positions
            this.Reset();
        }

        public override void Reset()
        {
            this.DesiredDistance = this.InitialDistance;
            this.MoveToDistance(this.InitialDistance);
        }

        protected override void Update(TimeSpan gameTime)
        {
            var elapsedMilliseconds = (float)gameTime.TotalMilliseconds;

            // We use FloatExtension Equal method to ensure the interpolation stops (uses a float error epsilon)
            if (!FloatExtensions.Equal(this.cameraTransform.LocalPosition.Z, this.DesiredDistance))
            {
                var res = MathHelper.SmoothDamp(
                    this.cameraTransform.LocalPosition.Z,
                    this.DesiredDistance,
                    ref this.smoothDampDeriv,
                    SmoothTimeMilliseconds,
                    elapsedMilliseconds);

                this.MoveToDistance(res);
            }

            if (!this.IsZoomRequested())
            {
                return;
            }

            this.HandleZoom();
        }

        private void MoveToDistance(float targetDistance)
        {
            if (this.cameraTransform != null)
            {
                var currentPosition = this.cameraTransform.LocalPosition;
                currentPosition.Z = targetDistance;
                this.cameraTransform.LocalPosition = currentPosition;
            }
        }

        private void HandleZoom()
        {
            if (!this.TryGetScrollPosition(out float scroll))
            {
                return;
            }

            this.DesiredDistance += this.ZoomFactor * scroll * (this.InverseScroll ? 1 : -1);
            this.DesiredDistance = MathHelper.Clamp(this.DesiredDistance, this.MinDistance, this.MaxDistance);
        }
    }
}
