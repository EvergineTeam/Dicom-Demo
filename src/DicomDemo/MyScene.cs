using Evergine.Common.IO;
using Evergine.Dicom;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Managers;
using Evergine.UI;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace DicomDemo
{
    public class MyScene : Scene
    {
        public Entity[] DicomEntities;
        public Transform3D[] Dicom2DTransforms;

        private bool isWasm = false;

        public override void RegisterManagers()
        {
            base.RegisterManagers();

            this.Managers.AddManager(new ImGuiManager()
            {
                ImGuizmoEnabled = true,
                ImPlotEnabled = true,
                ImNodesEnabled = true,
            });
        }

        protected override async void CreateScene()
        {
            this.SetCustomRenderPath();

            var dicomPath = new AssetsDirectory().RootPath + "/Dicoms/sample_dicom_2.zip";

            this.DicomEntities = new Entity[]
            {
                this.Managers.EntityManager.FindAllByTag("DICOM2DX").FirstOrDefault(),
                this.Managers.EntityManager.FindAllByTag("DICOM2DY").FirstOrDefault(),
                this.Managers.EntityManager.FindAllByTag("DICOM2DZ").FirstOrDefault(),
                this.Managers.EntityManager.FindAllByTag("DICOM3D").FirstOrDefault(),
            };

            this.Dicom2DTransforms = new Transform3D[]
            {
                this.DicomEntities[0].FindComponent<Transform3D>(),
                this.DicomEntities[1].FindComponent<Transform3D>(),
                this.DicomEntities[2].FindComponent<Transform3D>(),
             };

            var dicomComponent = this.DicomEntities[3].FindComponent<DicomComponent>();

            this.DicomEntities[0].FindComponent<Dicom2DViewComponent>().Dicom = dicomComponent;
            this.DicomEntities[1].FindComponent<Dicom2DViewComponent>().Dicom = dicomComponent;
            this.DicomEntities[2].FindComponent<Dicom2DViewComponent>().Dicom = dicomComponent;

            if (await dicomComponent.LoadFromFile(dicomPath))
            {
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
            cameraComponent.RenderPath = new DicomRenderPath((RenderManager)this.Managers.RenderManager);
        }

        public bool IsDicomEntityEnabled(int index)
        {
            Debug.Assert(index >= 0 && index < 4);
            return this.DicomEntities[index].IsEnabled;
        }

        public void SetDicomEntityEnabled(int index, bool enabled)
        {
            Debug.Assert(index >= 0 && index < 4);
            this.DicomEntities[index].IsEnabled = enabled;
        }

        private void createImguiBehavior()
        {
            var imguiComp = new ImguiBehavior(this);
            var imgui = new Entity().AddComponent(imguiComp);
            Managers.EntityManager.Add(imgui);
        }
    }
}


