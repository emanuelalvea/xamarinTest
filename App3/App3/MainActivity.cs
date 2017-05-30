using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Support.V4.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Graphics;
using Android.Animation;
using Android.Hardware;
using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using Android.Views.Animations;
using Android.Runtime;
using System;
using Android.Content;
using Android.Preferences;

namespace App3
{
    [Activity(Label = "StarMe App", MainLauncher = true, Icon = "@drawable/smartphone", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity, View.IOnTouchListener
    {
        DrawerLayout drawerLayout;
        ListView sideBarFrente;
        ListView sideBarFondo;
        Toolbar _toolbar;
        List<TableItem> frenteItems = new List<TableItem>();
        List<TableItem> fondoItems = new List<TableItem>();
        // animation vars
        ObjectAnimator objAnim;
        SensorManager _sensorManager;
        ImageView _imageView;
        private float mAccel; // acceleration apart from gravity
        private float mAccelCurrent; // current acceleration including gravity
        private float mAccelLast; // last acceleration including gravity
        MotionVector previousSensorValue;
        private long lastUpdate = 0;
        private double last_x, last_y, last_z;
        private const int SHAKE_THRESHOLD = 500;
        string starColorValue;
        string backColorValue;
        private const string ColorPrimary = "#37bf8f";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            MobileCenter.Start("717c564b-1e01-4a18-b39c-e2b994d255d0",
       typeof(Analytics), typeof(Crashes));

            SetContentView(Resource.Layout.Main);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);

            sideBarFrente = FindViewById<ListView>(Resource.Id.menu_frente);
            sideBarFondo = FindViewById<ListView>(Resource.Id.menu_fondo);

            sideBarFrente.Tag = 0;
            sideBarFondo.Tag = 1;

            _toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetActionBar(_toolbar);

            CreateFrenteItems();

            CreateFondoItems();

            sideBarFrente.Adapter = new HomeScreenAdapter(this, frenteItems);
            sideBarFrente.ItemClick += OnListItemClick;

            sideBarFondo.Adapter = new HomeScreenAdapter(this, fondoItems);
            sideBarFondo.ItemClick += OnListItemClick;

            View view = View.Inflate(this, Resource.Layout.title_sideBar, null);

            var back = view.FindViewById<TextView>(Resource.Id.title_side);
            back.Text = "Back Color";
            sideBarFondo.AddHeaderView(view);


            View viewFront = View.Inflate(this, Resource.Layout.title_sideBar, null);

            var front = viewFront.FindViewById<TextView>(Resource.Id.title_side);
            front.Text = "Star Color";
            sideBarFrente.AddHeaderView(viewFront);

            //animation
            mAccel = 0.00f;
            mAccelCurrent = SensorManager.GravityEarth;
            mAccelLast = SensorManager.GravityEarth;
            _sensorManager = (SensorManager)GetSystemService(Context.SensorService);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView);
            var backLayout = FindViewById<RelativeLayout>(Resource.Id.backLayout);

            var btnAni = FindViewById<Button>(Resource.Id.btnAni);
            var btnStop = FindViewById<Button>(Resource.Id.stop);

            objAnim = ObjectAnimator.OfPropertyValuesHolder(_imageView, PropertyValuesHolder.OfFloat("scaleX", 80.5f), PropertyValuesHolder.OfFloat("scaleY", 75.5f));

   
            //editor.PutString("StarColor", value.Data.ToString());
            //editor.PutString("BackColor", "#ffffff");
            //editor.Commit();
            int colorBack = Color.White;
            int colorStar = Color.ParseColor(ColorPrimary);
            ISharedPreferences preferences = PreferenceManager.GetDefaultSharedPreferences(this);
            starColorValue = preferences.GetString("StarColor", colorStar.ToString());
            backColorValue = preferences.GetString("BackColor", colorBack.ToString());

            
            _imageView.SetColorFilter(new Color(int.Parse(starColorValue)));
            _imageView.SetBackgroundColor(new Color(int.Parse(backColorValue)));
            drawerLayout.SetBackgroundColor(new Color(int.Parse(backColorValue)));

            backLayout.SetOnTouchListener(this);

            btnAni.Click += delegate
            {
                pulseAnimation();
                CrossDeviceMotion.Current.Start(DeviceMotion.Plugin.Abstractions.MotionSensorType.Accelerometer);

                _toolbar.Alpha = .1f;
                _toolbar.Visibility = ViewStates.Gone;
            };

            btnStop.Click += delegate
            {
                CrossDeviceMotion.Current.Stop(DeviceMotion.Plugin.Abstractions.MotionSensorType.Accelerometer);
                objAnim.Cancel();
                _toolbar.Alpha = 1f;
                _toolbar.Visibility = ViewStates.Visible;

            };

            Action myAction = () =>
            {
                _toolbar.Alpha = .1f;
                _toolbar.Visibility = ViewStates.Gone;
            };
            drawerLayout.PostDelayed(myAction, 8000);

            CrossDeviceMotion.Current.SensorValueChanged += (s, a) =>
            {
                var x = ((MotionVector)a.Value).X;
                var y = ((MotionVector)a.Value).Y;
                var z = ((MotionVector)a.Value).Z;
                mAccelLast = mAccelCurrent;
                mAccelCurrent = (float)Math.Sqrt((double)(x * x + y * y + z * z));
                //previousSensorValue = previousSensorValue == null ? (MotionVector)a.Value : previousSensorValue;
              
                float delta = mAccelCurrent - mAccelLast;
                mAccel = mAccel * 0.9f + delta;

                var aux = Math.Abs((long)mAccel * 7) > 0 ? Math.Abs((long)mAccel) * 7 : (long)-700;
                if (this.OnShakeGesture(x, y, z))
                {
                    objAnim.SetDuration(100);
                    objAnim.Start();
                }
                else if ((objAnim.CurrentPlayTime == 0 && objAnim.AnimatedFraction == 0 && !objAnim.IsStarted))
                {

                    objAnim.SetDuration(Math.Abs(300 - aux));
                    objAnim.Start();

                }
                //previousSensorValue = (MotionVector)a.Value;

            };

            WindowManagerLayoutParams windowManagerLayoutParams = new WindowManagerLayoutParams();
            windowManagerLayoutParams.CopyFrom(Window.Attributes);
            windowManagerLayoutParams.ScreenBrightness = 1f; //set screen to full brightness
            Window.Attributes = windowManagerLayoutParams;

            var alert = new AlertDialog.Builder(this);
            alert.SetView(LayoutInflater.Inflate(Resource.Layout.ModalColor, null));
            alert.SetTitle("User Guide");
            alert.SetNegativeButton("Ok", delegate {
                

                alert.Dispose();
                drawerLayout.OpenDrawer(sideBarFrente);

            });
            

            alert.SetOnDismissListener(new OnDismissListener(() =>
            {
                // just in case the user pressed the back button
                pulseAnimation();
            }));

            alert.Create().Show();


            CrossDeviceMotion.Current.Start(DeviceMotion.Plugin.Abstractions.MotionSensorType.Accelerometer);

        }

        private bool OnShakeGesture(double x, double y, double z)
        {
            var result = false;
            var timestamp = Java.Lang.JavaSystem.CurrentTimeMillis() / 1000;

            long curTime = timestamp;

            if ((curTime - lastUpdate) > 3)
            {
                long diffTime = (curTime - lastUpdate);
                lastUpdate = curTime;

                var move = Math.Abs(x + y + z - last_x - last_y - last_z);
                double speed = move / diffTime * 100;

                if (speed > SHAKE_THRESHOLD)
                {
                    result = true;
                }


            }
            last_x = x;
            last_y = y;
            last_z = z;
            return result;
        }

        private void CreateFondoItems()
        {
            fondoItems.Add(new TableItem() { Heading = "Blanco", Color = Color.WhiteSmoke, Frente = false });
            fondoItems.Add(new TableItem() { Heading = "Gris", Color = Color.DarkGray, Frente = false });
            fondoItems.Add(new TableItem() { Heading = "Negro", Color = Color.Black, Frente = false });
            fondoItems.Add(new TableItem() { Heading = "Rojo", Color = Color.Firebrick, Frente = false });
            fondoItems.Add(new TableItem() { Heading = "Rosa", Color = Color.DeepPink, Frente = false });
            fondoItems.Add(new TableItem() { Heading = "Amarillo", Color = Color.Gold, Frente = false });
        }

        private void CreateFrenteItems()
        {
            frenteItems.Add(new TableItem() { Heading = "Rojo", Color = Color.Red, Frente = true });
            frenteItems.Add(new TableItem() { Heading = "Azul", Color = Color.Blue, Frente = true });
            frenteItems.Add(new TableItem() { Heading = "Cyan", Color = Color.Cyan, Frente = true });
            frenteItems.Add(new TableItem() { Heading = "Verde", Color = Color.LimeGreen, Frente = true });
            frenteItems.Add(new TableItem() { Heading = "Amarillo", Color = Color.Yellow, Frente = true });
            frenteItems.Add(new TableItem() { Heading = "Naranja", Color = Color.Orange, Frente = true });
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            

            MenuInflater.Inflate(Resource.Layout.top_menu, menu);
            
            
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Toast.MakeText(this, item.TitleFormatted,
            //    ToastLength.Short).Show();

            //PopupWindow modal = new PopupWindow(LayoutInflater.Inflate(Resource.Layout.ModalColor, null), LayoutParams.MatchParent, LayoutParams.WrapContent, true);
            //modal.AttachedInDecor = true;
            //modal.ShowAtLocation(drawerLayout, GravityFlags.Top, 90, 260);

            if (drawerLayout.IsDrawerOpen(sideBarFondo))
            {
                drawerLayout.CloseDrawer(sideBarFondo);
                return base.OnOptionsItemSelected(item);
            }

            if (drawerLayout.IsDrawerOpen(sideBarFrente))
            {
                pulseAnimation();

                drawerLayout.CloseDrawer(sideBarFrente);
                return base.OnOptionsItemSelected(item);
            }

            if (item.ItemId == Resource.Id.menu_fondo)
            {
                drawerLayout.OpenDrawer(sideBarFondo);
            }
            else
            {
                drawerLayout.OpenDrawer(sideBarFrente);
            }

            return base.OnOptionsItemSelected(item);
        }

        protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
        {
            var listView = sender as ListView;
            List<TableItem> tableItems = new List<TableItem>();

            if (e.Position.Equals(0))
            {
                return;
            }

            if (listView.Id == sideBarFrente.Id)
            {
                tableItems = frenteItems;
            }
            else
            {
                tableItems = fondoItems;
            }

            var t = tableItems[e.Position - 1];

            ISharedPreferencesEditor editor = PreferenceManager.GetDefaultSharedPreferences(this).Edit();

            var star = FindViewById<ImageView>(Resource.Id.imageView);
            if (t.Frente)
            {
                star.SetColorFilter(t.Color);
                editor.PutString("StarColor", t.Color.ToArgb().ToString());

                Android.Widget.Toast.MakeText(this, "New Star Color!", Android.Widget.ToastLength.Short).Show();
            }
            else
            {
                star.SetBackgroundColor(t.Color);
                drawerLayout.SetBackgroundColor(t.Color);
                editor.PutString("BackColor", t.Color.ToArgb().ToString());

                Android.Widget.Toast.MakeText(this, "New Back Color!", Android.Widget.ToastLength.Short).Show();
            }
            editor.Commit();

            drawerLayout.CloseDrawer(listView);

        }

        private void pulseAnimation()
        {
            objAnim.RepeatCount = 1;
            objAnim.RepeatMode = ValueAnimatorRepeatMode.Reverse;
            objAnim.Start();
        }

        public bool OnTouch(View v, MotionEvent e)
        {


            


            Action myAction = () =>
            {
                if (drawerLayout.IsDrawerOpen(sideBarFrente) || drawerLayout.IsDrawerOpen(sideBarFondo))
                {
                    return;
                }
                _toolbar.Alpha = .1f;
                _toolbar.Visibility = ViewStates.Gone;
            };

            if (_toolbar.IsShown)
            {
                v.RemoveCallbacks(myAction);


            }

            //if (drawerLayout.IsDrawerOpen(sideBarFrente) || drawerLayout.IsDrawerOpen(sideBarFondo))
            //{
            //    _toolbar.Alpha = 1f;
            //    _toolbar.Visibility = ViewStates.Visible;
            //    v.RemoveCallbacks(myAction);
            //    return true;
            //}

            _toolbar.Alpha = 1f;
            _toolbar.Visibility = ViewStates.Visible;

            v.PostDelayed(myAction, 5000);



            return true;
        }

        private sealed class OnDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
        {
            private readonly Action action;

            public OnDismissListener(Action action)
            {
                this.action = action;
            }

            public void OnDismiss(IDialogInterface dialog)
            {
                this.action();
            }
        }
    }
}

