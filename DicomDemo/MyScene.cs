using DicomDemo.OrbitCamera;
using Evergine.Common.IO;
using Evergine.Dicom;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.Mathematics;
using Evergine.UI;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace DicomDemo
{
    public class MyScene : Scene
    {
        private Entity dicom3DEntity;
        private Entity dicom2DX;
        private Entity dicom2DY;
        private Entity dicom2DZ;

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
            this.SetCustomRenderPath();

            var dicomPath = new AssetsDirectory().RootPath + "/Dicoms/sample_dicom_2.zip";

            this.dicom3DEntity = this.Managers.EntityManager.FindAllByTag("DICOM3D").FirstOrDefault();
            var dicomComponent = this.dicom3DEntity.FindComponent<DicomComponent>();

            if (await dicomComponent.LoadFromFile(dicomPath))
            {
                this.SetupScale();
                this.createImguiBehavior();
            }
            else
            {
                Debug.Assert(false, "Error loading DICOM");
            }
        }

        private void SetCustomRenderPath()
        {
            // substitute camera RenderPath
            var cameraComponent = this.Managers.EntityManager.FindFirstComponentOfType<Camera>(isExactType: false);
            cameraComponent.RenderPath = new CustomRenderPath((RenderManager)this.Managers.RenderManager);
        }

        public bool IsDicomEntityEnabled(int index)
        {
            Debug.Assert(index >= 0 && index < 4);
            return false;
            //return this.dicomEntities[index].IsEnabled;
        }

        public void SetDicomEntityEnabled(int index, bool enabled)
        {
            Debug.Assert(index >= 0 && index < 4);
            // this.dicomEntities[index].IsEnabled = enabled;
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

        private void SetupScale()
        {
            
            var dicomScale = this.dicom3DEntity.FindComponent<Transform3D>().Scale;
            var dicomRadius = MathF.Max(dicomScale.X, MathF.Max(dicomScale.Y, dicomScale.Z));


            var rootTransform = this.Managers.EntityManager.FindFirstComponentOfType<Transform3D>(tag: "DICOMRoot");
            rootTransform.LocalScale = Vector3.One * (1 / dicomRadius);
        }
    }
}


