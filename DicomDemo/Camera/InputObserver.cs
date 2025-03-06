using Evergine.Common.Input.Keyboard;
using Evergine.Common.Input.Mouse;
using Evergine.Common.Input.Pointer;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DicomDemo.OrbitCamera
{
    public abstract class InputObserver : Behavior
    {
        protected Camera3D camera;

        protected KeyboardDispatcher keyboardDispatcher;

        protected MouseDispatcher mouseDispatcher;

        protected PointerDispatcher touchDispatcher;

        public string CameraTag { get; set; }

        private Camera3D GetCamera()
        {
            if (string.IsNullOrEmpty(this.CameraTag))
            {
                return (this.Managers.RenderManager as RenderManager).RegisteredCameras.Cast<Camera3D>().FirstOrDefault();
            }
            else
            {
                return this.Managers.EntityManager.FindFirstComponentOfType<Camera3D>(tag: this.CameraTag);
            }
        }

        protected override void OnActivated()
        {
            this.camera = this.GetCamera();

            var display = this.camera?.Display;

            if (display != null)
            {
                this.keyboardDispatcher = display.KeyboardDispatcher;
                this.mouseDispatcher = display.MouseDispatcher;
                this.touchDispatcher = display.TouchDispatcher;
            }

            base.OnActivated();
        }

        protected bool GetPointerPosition(out Vector2 position)
        {
            var display = this.camera.Display;

            if (this.mouseDispatcher.Points.Count > 0)
            {
                position = this.mouseDispatcher.Points[0].Position.ToVector2();
                return true;
            }

            if (this.touchDispatcher.Points.Count > 0)
            {
                position = this.touchDispatcher.Points[0].Position.ToVector2();
                return true;
            }

            position = this.mouseDispatcher.Position.ToVector2();
            return position != Vector2.Zero;
        }
    }
}
