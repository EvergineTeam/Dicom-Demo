using Evergine.Bindings.Imgui;
using Evergine.Bindings.Imguizmo;
using Evergine.Dicom;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;
using Evergine.UI;
using System;

namespace DicomDemo
{
    public unsafe class ImguiBehavior : Behavior
    {
        [BindComponent(source: BindComponentSource.Scene)]
        private DicomComponent dicom = null;

        private MyScene scene;

        private bool imguiDemoOpen = false;

        public ImguiBehavior(MyScene scene)
        {
            this.scene = scene;
        }

        protected override void Update(TimeSpan gameTime)
        {
            // Imgui
            if (this.imguiDemoOpen)
            {
                ImguiNative.igShowDemoWindow(this.imguiDemoOpen.Pointer());
            }

            this.dicomViews();
            this.dicomWindowRangeControls();
            this.dicomDitheringCheckBox();
        }

        private void dicomViews()
        {
            if (dicom != null)
            {
                var labels = new string[] { "2D - X", "2D - Y", "2D - Z", "3D" };
                for (int i = 0; i < 4; i++)
                {
                    byte enabled = (byte)(this.scene.IsDicomEntityEnabled(i) ? 1 : 0);
                    if(ImguiNative.igCheckbox(labels[i], &enabled))
                    {
                        this.scene.SetDicomEntityEnabled(i, enabled != 0);
                    }

                    if(i < 3)
                    {
                        var view = this.scene.dicomEntities[i].FindComponent<Transform3D>();
                        float val = view.LocalPosition[i];
                        ImguiNative.igSameLine(0, -1);
                        bool changed = ImguiNative.igSliderFloat($"##{labels[i]}", &val, -0.5f * dicom.SizeMM[i], +0.5f * dicom.SizeMM[i], "%f", 0);
                        if (changed)
                        {
                            Vector3 pos = view.LocalPosition;
                            pos[i] = val;
                            view.LocalPosition = pos;
                        }
                    }
                }
            }
        }

        private void dicomWindowRangeControls()
        {
            if (dicom != null)
            {
                //ImguiNative.igBegin("DICOM", null, 0);
                var range = dicom.WindowRange;
                bool changed = false;
                changed |= ImguiNative.igSliderFloat("Window Min", &range.X, dicom.LimitWindowRange.X, dicom.LimitWindowRange.Y, "%.3f", 0);
                changed |= ImguiNative.igSliderFloat("Window Max", &range.Y, dicom.LimitWindowRange.X, dicom.LimitWindowRange.Y, "%.3f", 0);
                if (changed)
                {
                    dicom.WindowRange = range;
                }
                //ImguiNative.igEnd();
            }
        }

        private void dicomDitheringCheckBox()
        {
            if (dicom != null)
            {
                byte ditheringEnabled = (byte)(dicom.DitheringEnabled ? 1 : 0);
                if (ImguiNative.igCheckbox("Dithering", &ditheringEnabled))
                {
                    dicom.DitheringEnabled = ditheringEnabled != 0;
                }
            }
        }

        private void cubeGuizmo()
        {
            var io = ImguiNative.igGetIO();
            ImguizmoNative.ImGuizmo_SetRect(0, 0, io->DisplaySize.X, io->DisplaySize.Y);

            var camera = this.Managers.RenderManager.ActiveCamera3D;
            Matrix4x4 view = camera.View;
            Matrix4x4 project = camera.Projection;

            ImguizmoNative.ImGuizmo_ViewManipulate(view.Pointer(), 2, Vector2.Zero, new Vector2(128, 128), 0x10101010);

            Matrix4x4.Invert(ref view, out Matrix4x4 iview);
            var translation = iview.Translation;
            var rotation = iview.Rotation;

            Vector3* r = &rotation;
            camera.Transform.LocalRotation = *r;

            Vector3* t = &translation;
            camera.Transform.LocalPosition = *t;
        }
    }
}
