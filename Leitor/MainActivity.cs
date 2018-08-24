using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Util;
using Android.Graphics;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using static Android.Gms.Vision.Detector;
using System.Text;

namespace Leitor
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        private SurfaceView sv;
        private CameraSource cs;
        private TextView tv;
        private const int RequestCameraPermissionID = 1001;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermissionID:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            cs.Start(sv.Holder);
                        }
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            sv = FindViewById<SurfaceView>(Resource.Id.surface);
            tv = FindViewById<TextView>(Resource.Id.txtV);


            TextRecognizer tr = new TextRecognizer.Builder(ApplicationContext).Build();
            if (!tr.IsOperational)
                Log.Error("Main Activity", "Naim");
            else
            {
                cs = new CameraSource.Builder(ApplicationContext, tr)
                    .SetFacing(CameraFacing.Back)
                    .SetRequestedPreviewSize(1280, 1024)
                    .SetRequestedFps(2.0f)
                    .SetAutoFocusEnabled(true)
                    .Build();

                sv.Holder.AddCallback(this);
                tr.SetProcessor(this);
            }




        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
           
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Android.Manifest.Permission.Camera

                }, RequestCameraPermissionID);
                return;
            }
            cs.Start(sv.Holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cs.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            SparseArray items = detections.DetectedItems;
            if(items.Size() != 0)
            {
                tv.Post(() =>
                {
                    StringBuilder sb = new StringBuilder();
                    for(int i = 0;i<items.Size(); ++i)
                    {
                        sb.Append(((TextBlock)items.ValueAt(i)).Value);
                        sb.Append("\n");
                    }
                    tv.Text = sb.ToString();
                }); 
            }
        }

        public void Release()
        {
            
        }
    }
}