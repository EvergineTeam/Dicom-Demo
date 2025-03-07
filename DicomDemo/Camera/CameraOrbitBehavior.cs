// Forked from Wave Engine's develop (ce51ee3f557fbf2ebbba2c27ea5282e73e39ed2a):
// src/Tools/Editor/WaveEngine.Runner/Viewers/Common/CameraBehavior.cs
// https://dev.azure.com/waveengineteam/Wave.Engine/_git/WaveEngine?path=%2Fsrc%2FTools%2FEditor%2FWaveEngine.Runner%2FViewers%2FCommon%2FCameraBehavior.cs

// Copyright © 2019 Wave Engine S.L. All rights reserved. Use is subject to license terms.

using System;
using System.Diagnostics;
using System.Linq;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace DicomDemo.OrbitCamera
{
    public abstract class CameraOrbitBehavior : InputObserver
    {
        private const int OrbitSmoothTimeMilliseconds = 50;

        /// <summary>
        /// The camera to move.
        /// </summary>
        [BindComponent(false)]
        public Transform3D Transform = null;

        protected Vector2 currentPointerPosition;

        protected Vector2 lastPointerPosition;

        protected bool isOrbiting;

        private Transform3D targetTransform;

        private Quaternion initialOrientation;
        private Quaternion targetInitialOrientation;

        private float objectInitialAngleRadians;

        private float theta;

        private float targetTheta;

        private float phi;

        private float targetPhi;

        private Quaternion objectOrbitSmoothDampDeriv;

        private Quaternion targetOrbitSmoothDampDeriv;


        public string PivotTag = "CameraPivot";

        public float OrbitFactor = 0.0025f;
        public float ForceOrbitTime = 1000;
        private float thetaVelocity;
        private float phiVelocity;
        private float ignoreNextOrbit;

        /// <summary>
        /// Reset camera position.
        /// </summary>
        public override void Reset()
        {
            this.targetTransform.LocalPosition = Vector3.Zero;
            this.targetTransform.LocalRotation = Vector3.Zero;
            this.Transform.LocalPosition = Vector3.Zero;
            this.Transform.LocalRotation = Vector3.Zero;

            this.theta = 0;


            this.targetTheta = this.theta;
            this.targetPhi = this.phi;

            this.isOrbiting = false;
        }

        public void ResetInertia()
        {
            this.targetTransform.LocalOrientation = this.targetInitialOrientation;
            this.Transform.LocalOrientation = this.initialOrientation;

            this.theta = this.targetTransform.LocalRotation.Y;
            this.phi = -this.Transform.LocalRotation.X;

            this.targetTheta = this.theta;
            this.targetPhi = this.phi;

            this.isOrbiting = false;
        }

        /// <inheritdoc/>
        protected override bool OnAttached()
        {
            var child = this.Owner.FindChildrenByTag(this.PivotTag).FirstOrDefault();
            this.targetTransform = child.FindComponent<Transform3D>();
            this.targetInitialOrientation = this.targetTransform.LocalOrientation;

            this.initialOrientation = this.Transform.LocalOrientation;
            this.objectInitialAngleRadians = 0;

            this.theta = this.targetTransform.LocalRotation.Y;
            this.phi = -this.Transform.LocalRotation.X;

            this.targetTheta = this.theta;
            this.targetPhi = this.phi;

            return base.OnAttached();
        }

        /// <inheritdoc/>
        protected override void Update(TimeSpan gameTime)
        {
            if (this.TryGetPointerPosition(out Vector2 position))
            {
                this.lastPointerPosition = this.currentPointerPosition;
                this.currentPointerPosition = position;
            }
            else
            {
                return;
            }

            float elapsedMilliseconds = (float)gameTime.TotalMilliseconds;

            this.HandleOrbit(elapsedMilliseconds);

            if (this.isOrbiting)
            {
                this.theta = MathHelper.SmoothDamp(this.theta, this.targetTheta, ref this.thetaVelocity, OrbitSmoothTimeMilliseconds * 5, elapsedMilliseconds);
                this.phi = MathHelper.SmoothDamp(this.phi, this.targetPhi, ref this.phiVelocity, OrbitSmoothTimeMilliseconds * 5, elapsedMilliseconds);

                var orientation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, this.theta);
                this.Transform.LocalOrientation = Quaternion.SmoothDamp(
                    this.Transform.LocalOrientation,
                    orientation,
                    ref this.objectOrbitSmoothDampDeriv,
                    OrbitSmoothTimeMilliseconds,
                    elapsedMilliseconds);

                var targetOrientation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, -this.phi);
                this.targetTransform.LocalOrientation = Quaternion.SmoothDamp(
                    this.targetTransform.LocalOrientation,
                    targetOrientation,
                    ref this.targetOrbitSmoothDampDeriv,
                    OrbitSmoothTimeMilliseconds,
                    elapsedMilliseconds);

                if (this.targetTransform.LocalOrientation == orientation &&
                    this.Transform.LocalOrientation == targetOrientation)
                {
                    this.isOrbiting = false;
                }
            }
        }

        public void OrbitTo(float cameraOrientation)
        {
            this.targetTheta = MathHelper.ToRadians(cameraOrientation);
            this.targetPhi = 0;

            this.isOrbiting = true;

            this.ignoreNextOrbit = this.ForceOrbitTime;
        }

        protected abstract bool TryGetPointerPosition(out Vector2 position);

        protected virtual bool IsOrbitRequested() => false;

        protected Vector2 CalcDelta(Vector2 current, Vector2 last)
        {
            Vector2 delta;
            delta.X = -current.X + last.X;
            delta.Y = current.Y - last.Y;

            return delta;
        }

        private void HandleOrbit(float elapsedMilliseconds)
        {
            if (this.ignoreNextOrbit > 0)
            {
                this.ignoreNextOrbit -= elapsedMilliseconds;

                return;
            }

            if (!this.IsOrbitRequested())
            {
                return;
            }

            Vector2 delta = -this.CalcDelta(this.currentPointerPosition, this.lastPointerPosition);

            delta *= OrbitFactor;

            this.theta -= delta.X;
            this.phi -= delta.Y;

            if (this.phi > (MathHelper.PiOver2 + this.objectInitialAngleRadians))
            {
                this.phi = MathHelper.PiOver2 + this.objectInitialAngleRadians;
            }
            else if (this.phi < -(MathHelper.PiOver2 - this.objectInitialAngleRadians))
            {
                this.phi = -(MathHelper.PiOver2 - this.objectInitialAngleRadians);
            }


            this.targetTheta = this.theta;
            this.targetPhi = this.phi;

            this.isOrbiting = true;
        }
    }
}
