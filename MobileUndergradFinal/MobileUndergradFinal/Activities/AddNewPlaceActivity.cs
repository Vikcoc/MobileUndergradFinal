using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using Communication.SourceVariantDto;
using Google.Android.Material.BottomSheet;
using MobileUndergradFinal.AdapterDto;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.Helper;
using MobileUndergradFinal.ItemDecorators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "AddNewPlaceActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddNewPlaceActivity : TokenAndErrorActivity, IOnMapReadyCallback, IAddNewFountainScreen
    {
        private GoogleMap _map;
        private string _address;
        private decimal _latitude;
        private decimal _longitude;

        private View _cover;
        private View _text;

        private View _selectedView;
        private Button _selectVariant;

        private BottomSheetDialog _dialog;
        private FountainsTypeAdapter _fountainsTypeAdapter;

        private readonly AddNewFountainLogic _logic;

        public Guid? SelectedVariant { get; private set; }

        private PlacePictureAdapter _pictureAdapter;

        private Uri _photoURI;

        public AddNewPlaceActivity()
        {
            _logic = new AddNewFountainLogic(this);
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.add_new_place);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            var mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            _cover = FindViewById<View>(Resource.Id.mapCover);
            _cover.Click += (sender, args) =>
            {
                StartActivityForResult(typeof(MyMapsActivity), 1);
            };
            _text = FindViewById<TextView>(Resource.Id.mapClick);

            _selectVariant = FindViewById<Button>(Resource.Id.selectVariantButton);
            _selectVariant.Click += (sender, args) => ShowDialog();


            _selectedView = FindViewById(Resource.Id.selected);
            _selectedView.Click += (sender, args) => ShowDialog();


            var pictureButton = FindViewById<Button>(Resource.Id.addPictures);
            pictureButton.Click += PictureButtonClick;

            var recycler = FindViewById<RecyclerView>(Resource.Id.yourPictures);
            recycler.SetLayoutManager(new GridLayoutManager(this, 3, (int)Orientation.Vertical, false));
            _pictureAdapter = new PlacePictureAdapter();
            recycler.SetAdapter(_pictureAdapter);
        }

        public void PictureButtonClick(object sender, EventArgs args)
        {
            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted
                || ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted
                || ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.Camera) != (int)Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this,
                    new[]
                    {
                        Android.Manifest.Permission.ReadExternalStorage,
                        Android.Manifest.Permission.WriteExternalStorage,
                        Android.Manifest.Permission.Camera
                    }, 10);
            }
            else
            {
                var intents = new List<Intent>();
                intents.AddRange(GetCameraIntents());
                intents.AddRange(GetGalleryIntents());
                if (intents.Any())
                    StartPicturesForResult(intents);
            }
        }

        public async void ShowDialog()
        {
            if (_dialog == null)
            {
                _dialog = new BottomSheetDialog(this);
                _dialog.SetContentView(Resource.Layout.select_variant_dialog_layout);

                var recycler = _dialog.FindViewById<RecyclerView>(Resource.Id.fountainType);
                _fountainsTypeAdapter = new FountainsTypeAdapter(variant =>
                {
                    SelectedVariant = variant.Id;
                    var topText = _selectedView.FindViewById<TextView>(Resource.Id.textView4);
                    topText.Text = variant.Name;
                    var bottomText = _selectedView.FindViewById<TextView>(Resource.Id.textView5);
                    bottomText.Text = variant.Description;
                    if (variant.Picture != null)
                        _selectedView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageBitmap(variant.Picture);

                    _selectedView.Visibility = ViewStates.Visible;
                    _selectVariant.Visibility = ViewStates.Invisible;

                    _dialog.Dismiss();
                });
                recycler.SetLayoutManager(new LinearLayoutManager(_dialog.Context));
                recycler.SetAdapter(_fountainsTypeAdapter);
                recycler.AddItemDecoration(new FountainTypesDecorator(Resources.GetDimensionPixelOffset(Resource.Dimension.margin_small)));
                await _logic.RequestVariants();
            }
            _dialog.Show();
        }
        
        public void SetWaterSourceVariants(List<WaterSourceVariantDto> waterSources)
        {
            _fountainsTypeAdapter.AddItems(waterSources.Select(x => new WaterSourceVariant
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name
            }).ToList());
        }

        public void AddPicture(Guid variantId, Stream picture)
        {
            var bitmap = BitmapHelper.GetOfScale(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                picture);
            _fountainsTypeAdapter.AddPicture(variantId, bitmap);
            if (SelectedVariant.HasValue && SelectedVariant.Value == variantId)
                _selectedView.FindViewById<ImageView>(Resource.Id.imageView2).SetImageBitmap(bitmap);

        }

        public async void OnMapReady(GoogleMap map)
        {
            _map = map;

            _map.UiSettings.MyLocationButtonEnabled = false;

            if (this.CheckSelfPermission(Android.Manifest.Permission.AccessFineLocation) == (int)Permission.Granted && this.CheckSelfPermission(Android.Manifest.Permission.AccessCoarseLocation) == (int)Permission.Granted)
            {
                map.MyLocationEnabled = true;

                var locationProvider = LocationServices.GetFusedLocationProviderClient(this);
                var x = await locationProvider.GetLastLocationAsync();

                map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(x.Latitude, x.Longitude), 17));
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            switch (resultCode)
            {
                case Result.Ok when requestCode == 1:
                {
                    _address = data.GetStringExtra("address");
                    _latitude = Convert.ToDecimal(data.GetDoubleExtra("latitude", 0));
                    _longitude = Convert.ToDecimal(data.GetDoubleExtra("longitude", 0));
                    var fountainPosition =
                        new LatLng(data.GetDoubleExtra("latitude", 0), data.GetDoubleExtra("longitude", 0));
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(fountainPosition, 17));
                    _text.Visibility = ViewStates.Gone;
                    _cover.Background = null;
                    _map.AddMarker(new MarkerOptions()
                        .SetPosition(fountainPosition)
                        .SetTitle("Selected position"));
                    break;
                }
                case Result.Ok when requestCode == 2 && data != null && (data.Data != null || data.ClipData != null):
                {
                    if(data.Data == null)
                    {
                        var res = data.ClipData;

                        var pictures = new List<(Uri, Bitmap)>(res.ItemCount);

                        for (var i = 0; i < res.ItemCount; i++)
                        {
                            var uri = res.GetItemAt(i).Uri;
                            var stream = ContentResolver.OpenInputStream(uri);
                            var bitmap = BitmapHelper.GetRotated(
                                Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                                Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                                stream);
                            pictures.Add((uri, bitmap));
                        }
                        _pictureAdapter.AddImages(pictures);
                    }
                    else
                    {
                        var stream = ContentResolver.OpenInputStream(data.Data);
                        var bitmap = BitmapHelper.GetRotated(
                            Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                            Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                            stream);
                        _pictureAdapter.AddImages(new List<(Uri, Bitmap)>{(data.Data, bitmap) });
                    }

                    break;
                }
                case Result.Ok when requestCode == 2 && (data == null || data.Data == null && data.ClipData == null):
                {
                    var stream = ContentResolver.OpenInputStream(_photoURI);
                    var bitmap = BitmapHelper.GetRotated(
                        Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                        Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                        stream);
                    _pictureAdapter.AddImages(new List<(Uri, Bitmap)> { (_photoURI, bitmap) });
                    break;
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            var intents = new List<Intent>();

            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted
            && ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.Camera) == (int)Permission.Granted)
            {
                intents.AddRange(GetCameraIntents());
            }

            if (ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) ==
                (int)Permission.Granted)
            {
                intents.AddRange(GetGalleryIntents());
            }

            if (intents.Any())
                StartPicturesForResult(intents);
        }

        private List<Intent> GetGalleryIntents()
        {
            var galleryIntent = new Intent(Intent.ActionGetContent);
            galleryIntent.SetType("image/*");
            galleryIntent.PutExtra(Intent.ExtraAllowMultiple, true);
            var allIntents = new List<Intent>();

            IList<ResolveInfo> listGallery = this.PackageManager.QueryIntentActivities(galleryIntent, 0);
            foreach (var res in listGallery)
            {
                Intent intent = new Intent(galleryIntent);
                intent.SetComponent(new ComponentName(res.ActivityInfo.PackageName, res.ActivityInfo.Name));

                intent.SetPackage(res.ActivityInfo.PackageName);
                allIntents.Add(intent);
            }

            return allIntents;
        }

        private List<Intent> GetCameraIntents()
        {
            //var SD_CARD_TEMP_DIR = Environment.ExternalStorageDirectory + Java.IO.File.Separator + DateTime.Now.ToString("hh_mm_ss") + ".jpg"; // Get File Path
            //var takePictureFromCameraIntent = new Intent(Android.Provider.MediaStore.ActionImageCapture);
            //takePictureFromCameraIntent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(new File(SD_CARD_TEMP_DIR)));
            //StartActivityForResult(takePictureFromCameraIntent, 123);
            //return new List<Intent> { takePictureFromCameraIntent };

            var file = new File(this.GetExternalFilesDir(Environment.DirectoryPictures), $"capture_{System.Guid.NewGuid().ToString().Substring(0, 5)}.jpg");
            var photoURI = FileProvider.GetUriForFile(this, this.PackageName + ".fileprovider", file);
            _photoURI = photoURI;

            var allIntents = new List<Intent>();

            // camera
            var captureIntent = new Intent(MediaStore.ActionImageCapture);
            captureIntent.PutExtra(MediaStore.ExtraOutput, photoURI);
            var listCam = this.PackageManager.QueryIntentActivities(captureIntent, 0);
            foreach (var res in listCam)
            {
                var intent = new Intent(captureIntent);
                intent.SetComponent(new ComponentName(res.ActivityInfo.PackageName, res.ActivityInfo.Name));
                intent.SetPackage(res.ActivityInfo.PackageName);
                allIntents.Add(intent);
            }

            return allIntents;
        }

        private void StartPicturesForResult(List<Intent> allIntents)
        {
            var chooserIntent = Intent.CreateChooser(allIntents.Last(), "Select source");

            //Add all other intents
            chooserIntent.PutExtra(Intent.ExtraInitialIntents, allIntents.ToArray());
            this.StartActivityForResult(chooserIntent, 2);
        }
    }
}