using Evergine.Common.Input.Keyboard;
using Evergine.Common.Input.Mouse;
using Evergine.Common.Input.Pointer;
using Evergine.Framework.Graphics;
using Evergine.Framework;

namespace DicomDemo.OrbitCamera
{
    public abstract class InputObserver : Behavior
    {
        protected Camera3D Camera;

        protected KeyboardDispatcher KeyboardDispatcher;

        protected MouseDispatcher MouseDispatcher;

        protected PointerDispatcher TouchDispatcher;

        ////public Vector2 ScreenSize;
        private Display display;

        public string CameraTag;

        protected override void OnActivated()
        {
            this.Camera = this.Managers.RenderManager.ActiveCamera3D;

            this.display = this.Camera?.Display;

            if (this.display != null)
            {
                this.KeyboardDispatcher = this.display.KeyboardDispatcher;
                this.MouseDispatcher = this.display.MouseDispatcher;
                this.TouchDispatcher = this.display.TouchDispatcher;
            }

            base.OnActivated();
        }

        public abstract void Reset();
    }
}
