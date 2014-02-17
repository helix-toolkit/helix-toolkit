
namespace DemoCore
{
    using System;
    using System.Collections.Generic;

    using HelixToolkit.Wpf.SharpDX;

    /// <summary>
    /// Base ViewModel for Demo Applications?
    /// </summary>
    public abstract class BaseViewModel : ObservableObject
    {
        public const string Orthographic = "Orthographic Camera";

        public const string Perspective = "Perspective Camera";

        private string cameraModel;

        private Camera camera;

        private RenderTechnique renderTechnique;

        private string subTitle;

        private string title;

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.SetValue(ref title, value, "Title");
            }
        }

        public string SubTitle
        {
            get
            {
                return this.subTitle;
            }
            set
            {
                this.SetValue(ref subTitle, value, "SubTitle");
            }
        }

        public RenderTechnique RenderTechnique
        {
            get
            {
                return this.renderTechnique;
            }
            set
            {
                this.SetValue(ref renderTechnique, value, "RenderTechnique");
            }
        }

        public List<string> ShadingModelCollection { get; private set; }

        public List<string> CameraModelCollection { get; private set; }

        public string CameraModel
        {
            get
            {
                return this.cameraModel;
            }
            set
            {
                if (this.SetValue(ref cameraModel, value, "CameraModel"))
                {
                    OnCameraModelChanged();
                }
            }
        }

        public Camera Camera
        {
            get
            {
                return this.camera;
            }

            protected set
            {
                this.SetValue(ref this.camera, value, "Camera");
                this.CameraModel = value is PerspectiveCamera
                                       ? Perspective
                                       : value is OrthographicCamera ? Orthographic : null;
            }
        }

        protected OrthographicCamera defaultOrthographicCamera = new OrthographicCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 1, FarPlaneDistance = 100 };

        protected PerspectiveCamera defaultPerspectiveCamera = new PerspectiveCamera { Position = new System.Windows.Media.Media3D.Point3D(0, 0, 5), LookDirection = new System.Windows.Media.Media3D.Vector3D(-0, -0, -5), UpDirection = new System.Windows.Media.Media3D.Vector3D(0, 1, 0), NearPlaneDistance = 0.5, FarPlaneDistance = 150 };

        public event EventHandler CameraModelChanged;

        protected BaseViewModel()
        {
            // camera models
            CameraModelCollection = new List<string>()
            {
                Orthographic,
                Perspective,
            };

            // on camera changed callback
            this.CameraModelChanged += (s, e) =>
            {
                if (this.cameraModel == Orthographic)
                {
                    //if (this.Camera != null)
                    //{
                    //    var newCamera = new OrthographicCamera();
                    //    this.Camera.CopyTo(newCamera);
                    //    newCamera.NearPlaneDistance = znear;
                    //    newCamera.FarPlaneDistance = zfar;
                    //    this.Camera = newCamera;

                    //}
                    //else
                    {
                        this.Camera = this.defaultOrthographicCamera;
                    }
                }
                else if (this.cameraModel == Perspective)
                {
                    //if (this.Camera != null)
                    //{
                    //    var newCamera = new PerspectiveCamera();
                    //    this.Camera.CopyTo(newCamera);
                    //    newCamera.NearPlaneDistance = znear;
                    //    newCamera.FarPlaneDistance = zfar;
                    //    this.Camera = newCamera;
                    //}
                    //else
                    {
                        this.Camera = this.defaultPerspectiveCamera;
                    }
                }
                else
                {
                    throw new HelixToolkitException("Camera Model Error.");
                }
            };

            // default camera model
            this.CameraModel = Perspective;

            this.Title = "Demo (HelixToolkitDX)";
            this.SubTitle = "Default Base View Model";
            this.RenderTechnique = Techniques.RenderPhong;
        }

        protected virtual void OnCameraModelChanged()
        {
            var eh = this.CameraModelChanged;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }
        }
    }
}
