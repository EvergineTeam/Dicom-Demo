using DicomDemo.OrbitCamera;
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
        //public Entity[] dicomEntities = new Entity[4]; // 0: 2D-X, 1: 2D-Y, 2: 2D-Z, 3: 3D
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
            // substitute camera RenderPath
            var cameraComponent = this.Managers.EntityManager.FindFirstComponentOfType<Camera>(isExactType: false);
            cameraComponent.RenderPath = new CustomRenderPath((RenderManager)this.Managers.RenderManager);


            var dicomPath = new AssetsDirectory().RootPath + "/Dicoms/sample_dicom_2.zip";
            var dicomComponent = this.Managers.EntityManager.FindFirstComponentOfType<DicomComponent>();
            var success = await dicomComponent.LoadFromFile(dicomPath);

            //this.dicomEntities = await this.CreateDicomEntities(dicomPath, true, true, true);

            if (success)
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

        private void setupCamera()
        {
            ////var dicomScale = this.dicomEntities[3].FindComponent<Transform3D>().Scale;
            ////var cameraDistance = 1.1f * MathF.Max(dicomScale.X, MathF.Max(dicomScale.Y, dicomScale.Z));

            ////var orbitCamera = this.Managers.EntityManager.FindComponentsOfType<TouchAndMouseOrbitBehavior>();
            ////orbitCamera.s

            ////var cameraEntity = this.Managers.EntityManager.Find("Camera");
            ////var transform = cameraEntity.FindComponent<Transform3D>();
            ////transform.Position = new Vector3(0, 0, cameraDistance);
            ////transform.Orientation = Quaternion.Identity;
        }
    }
}


