using System;
using System.Diagnostics;
using System.Linq;
using Evergine.Common.Input;
using Evergine.Common.Input.Keyboard;
using Evergine.Common.Input.Mouse;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace DicomDemo.OrbitCamera
{
    public class TouchAndMouseOrbitBehavior : CameraOrbitBehavior
    {
        private static readonly TimeSpan maxTimeBetweenResetPoints = TimeSpan.FromMilliseconds(150);

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override bool TryGetPointerPosition(out Vector2 position)
        {
            position = default;

            if ((this.TouchDispatcher != null) && (this.TouchDispatcher.Points.Count > 0))
            {
                if (this.TouchDispatcher.Points.Count == 1)
                {
                    position = this.TouchDispatcher.Points[0].Position.ToVector2();
                }
                else if (this.TouchDispatcher.Points.Count == 2 &&
                    this.TouchDispatcher.Points.Any(item => item.State == ButtonState.Releasing) &&
                    this.TouchDispatcher.Points.Any(item => item.State == ButtonState.Pressed))
                {
                    position = this.TouchDispatcher.Points
                        .First(item => item.State == ButtonState.Pressed)
                        .Position.ToVector2();
                }
                else
                {
                    position = this.lastPointerPosition;
                }
            }
            else if (this.MouseDispatcher != null)
            {
                position = this.MouseDispatcher.Position.ToVector2();
            }
            else
            {
                return false;
            }

            return true;
        }

        protected override bool IsOrbitRequested()
        {
            var isRequested = this.TouchDispatcher.Points.Count == 1;

            if (isRequested && this.TouchDispatcher.Points[0].State == ButtonState.Pressing)
            {
                this.lastPointerPosition = this.currentPointerPosition;
            }

            if(!isRequested)
            {
                isRequested = this.MouseDispatcher.IsButtonDown(MouseButtons.Left) || this.MouseDispatcher.IsButtonDown(MouseButtons.Right);
            }

            return isRequested;
        }
    }
}