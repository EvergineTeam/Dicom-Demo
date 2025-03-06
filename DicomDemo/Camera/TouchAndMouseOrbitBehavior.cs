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
    public class TouchAndMouseOrbitBehavior : OrbitBehavior
    {
        private static readonly TimeSpan maxTimeBetweenResetPoints = TimeSpan.FromMilliseconds(150);

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override bool TryGetPointerPosition(out Vector2 position)
        {
            position = default;

            if ((this.touchDispatcher != null) && (this.touchDispatcher.Points.Count > 0))
            {
                if (this.touchDispatcher.Points.Count == 1)
                {
                    position = this.touchDispatcher.Points[0].Position.ToVector2();
                }
                else if (this.touchDispatcher.Points.Count == 2 &&
                    this.touchDispatcher.Points.Any(item => item.State == ButtonState.Releasing) &&
                    this.touchDispatcher.Points.Any(item => item.State == ButtonState.Pressed))
                {
                    position = this.touchDispatcher.Points
                        .First(item => item.State == ButtonState.Pressed)
                        .Position.ToVector2();
                }
                else
                {
                    position = this.lastPointerPosition;
                }
            }
            else if (this.mouseDispatcher != null)
            {
                position = this.mouseDispatcher.Position.ToVector2();
            }
            else
            {
                return false;
            }

            return true;
        }

        protected override bool IsOrbitRequested()
        {
            var isRequested = this.touchDispatcher.Points.Count == 1;

            if (isRequested && this.touchDispatcher.Points[0].State == ButtonState.Pressing)
            {
                this.lastPointerPosition = this.currentPointerPosition;
            }

            if(!isRequested)
            {
                isRequested = this.mouseDispatcher.IsButtonDown(MouseButtons.Left) || this.mouseDispatcher.IsButtonDown(MouseButtons.Right);
            }

            return isRequested;
        }
    }
}