using Evergine.Common.Input;
using Evergine.Common.Input.Keyboard;
using Evergine.Common.Input.Mouse;

namespace DicomDemo.OrbitCamera
{
    public class TouchAndMouseZoomBehavior : CameraZoomBehavior
    {
        public float TouchSensibility = 0.01f;

        private float lastDistance;

        protected override bool TryGetScrollPosition(out float scroll)
        {
            scroll = default;

            if (this.TouchDispatcher != null
                && this.TouchDispatcher.Points.Count == 2)
            {
                var touch1 = this.TouchDispatcher.Points[0];
                var touch2 = this.TouchDispatcher.Points[1];
                float distance = 0;

                if (touch1.Position.X != touch2.Position.X
                    || touch1.Position.Y != touch2.Position.Y)
                {
                    distance = (touch2.Position - touch1.Position).ToVector2().Length();
                }

                scroll = (this.lastDistance - distance) * this.TouchSensibility;

                this.lastDistance = distance;
            }
            else if (this.MouseDispatcher != null)
            {
                scroll = this.MouseDispatcher.ScrollDelta.Y;
            }
            else
            {
                return false;
            }

            return true;
        }

        protected override bool IsZoomRequested()
        {
            var isRequested = this.TouchDispatcher.Points.Count == 2;

            if (isRequested
                && (this.TouchDispatcher.Points[0].State == ButtonState.Pressing
                    || this.TouchDispatcher.Points[1].State == ButtonState.Pressing))
            {
                this.lastDistance = (this.TouchDispatcher.Points[1].Position - this.TouchDispatcher.Points[0].Position).ToVector2().Length();
            }

            if (!isRequested)
            {
                isRequested = !this.MouseDispatcher.IsButtonDown(MouseButtons.Left)
                    && !this.KeyboardDispatcher.IsKeyDown(Keys.LeftShift)
                    && !this.KeyboardDispatcher.IsKeyDown(Keys.RightShift)
                    && !this.KeyboardDispatcher.IsKeyDown(Keys.LeftControl)
                    && !this.KeyboardDispatcher.IsKeyDown(Keys.RightControl)
                    && this.MouseDispatcher.ScrollDelta.Y != 0;
            }

            return isRequested;
        }
    }
}
