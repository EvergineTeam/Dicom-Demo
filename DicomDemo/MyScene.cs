using Evergine.Common.IO;
using Evergine.Dicom;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Mathematics;
using Evergine.UI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DicomDemo
{
    public class MyScene : Scene
    {
        public Entity[] dicomEntities = new Entity[4]; // 0: 2D-X, 1: 2D-Y, 2: 2D-Z, 3: 3D
        private CustomRenderPath renderPath;
        private bool isWasm = false;

        public override void RegisterManagers()
        {
            base.RegisterManagers();

            this.Managers.AddManager(new global::Evergine.Bullet.BulletPhysicManager3D());
            this.isWasm = RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY"));
            if (!this.isWasm)
            {
                this.Managers.AddManager(new ImGuiManager()
                {
                    ImGuizmoEnabled = true,
                    ImPlotEnabled = true,
                    ImNodesEnabled = true,
                });
            }
        }

        protected override async void CreateScene()
        {
            if (true)
            {
                // substitute camera RenderPath
                var cameraEntity = this.Managers.EntityManager.Find("Camera");
                var cameraComponent = cameraEntity.FindComponent<Camera>(isExactType: false);
                cameraComponent.RenderPath = new CustomRenderPath((RenderManager)this.Managers.RenderManager);
            }
            else
            {
                // substitute pipeline renderPath
                this.renderPath = new CustomRenderPath((RenderManager)this.Managers.RenderManager);
                var renderPipeline = this.Managers.RenderManager.RenderPipeline;
                renderPipeline.RemoveRenderPath(renderPipeline.DefaultRenderPath);
                renderPipeline.AddRenderPath(this.renderPath);
            }

            var dicomPath = new AssetsDirectory().RootPath + "/Dicoms/sample_dicom_2.zip";

            this.dicomEntities = await this.CreateDicomEntities(dicomPath, true, true, true);

            if (this.dicomEntities.Length == 4)
            {
                this.setupCamera();
                this.createImguiBehavior();
            }
            else
            {
                Debug.Assert(false, "Error loading DICOM");
            }
        }

        public bool IsDicomEntityEnabled(int index)
        {
            Debug.Assert(index >= 0 && index < 4);
            return this.dicomEntities[index].IsEnabled;
        }

        public void SetDicomEntityEnabled(int index, bool enabled)
        {
            Debug.Assert(index >= 0 && index < 4);
            this.dicomEntities[index].IsEnabled = enabled;
        }

        private void createImguiBehavior()
        {
            if (!this.isWasm)
            {
                var imguiComp = new ImguiBehavior(this);
                var imgui = new Entity().AddComponent(imguiComp);
                Managers.EntityManager.Add(imgui);
            }
        }

        private void setupCamera()
        {
            var dicomScale = this.dicomEntities[3].FindComponent<Transform3D>().Scale;
            var cameraDistance = 1.1f * MathF.Max(dicomScale.X, MathF.Max(dicomScale.Y, dicomScale.Z));

            var cameraEntity = this.Managers.EntityManager.Find("Camera");
            var transform = cameraEntity.FindComponent<Transform3D>();
            transform.Position = new Vector3(0, 0, cameraDistance);
            transform.Orientation = Quaternion.Identity;
        }
    }
}


