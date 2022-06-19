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
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using BusinessLogic.Dashboard;
using Communication.SourceVariantDto;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.TextField;
using MobileUndergradFinal.AdapterDto;
using MobileUndergradFinal.Adapters;
using MobileUndergradFinal.Helper;
using MobileUndergradFinal.ItemDecorators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Environment = Android.OS.Environment;
using File = Java.IO.File;
using Uri = Android.Net.Uri;

namespace MobileUndergradFinal.Activities
{
    [Activity(Label = "AddNewPlaceActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class AddNewPlaceActivity : TokenAndErrorActivity, IOnMapReadyCallback, IAddNewFountainScreen
    {
        private GoogleMap _map;

        private TextView _nickname;
        private TextInputLayout _nicInputLayout;

        private View _cover;
        private View _text;

        private View _selectedView;
        private Button _selectVariant;

        private Button _pictureButton;

        private BottomSheetDialog _dialog;
        private FountainsTypeAdapter _fountainsTypeAdapter;

        private readonly AddNewFountainLogic _logic;

        public Guid? VariantId { get; private set; }

        public string Nickname => _nickname.Text;

        public string Address { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public List<Stream> Pictures {
            get
            {
                var x =_pictureAdapter.Pictures.Select(x => ContentResolver.OpenInputStream(x).GetRotated(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                        Resources.GetDimensionPixelSize(Resource.Dimension.image_size)))
                    .Select(x =>
                    {
                        var str = new MemoryStream();
                        x.WriteToStream(str);
                        str.Seek(0, SeekOrigin.Begin);
                        return (Stream) str;
                    })
                    .ToList();
                return x;
            }
        }


        public string NicknameError
        {
            set => _nicInputLayout.Error = value;
        }

        public string MapError
        {
            set
            {
                var text = FindViewById<TextView>(Resource.Id.mapErrorText);
                if (string.IsNullOrWhiteSpace(value))
                {
                    text.Visibility = ViewStates.Invisible;
                    _cover.SetBackgroundColor(new Color(0,0,0,112));
                }
                else
                {
                    _cover.Background = Resources.GetDrawable(Resource.Drawable.error_outline, Theme);
                    text.Text = value;
                    text.Visibility = ViewStates.Visible;
                }

            }
        }

        public string VariantError
        {
            set => _selectVariant.Error = value;
        }
        public string PicturesError
        {
            set => _pictureButton.Error = value;
        }

        public Func<Task> OnSubmitButtonPress { get; set; }
        public string NicknameErrorText => Resources.GetString(Resource.String.add_place_nickname_error);
        public string MapErrorText => Resources.GetString(Resource.String.add_place_map_error);
        public string VariantErrorText => Resources.GetString(Resource.String.add_place_variant_error);
        public string PicturesErrorText => Resources.GetString(Resource.String.add_place_pictures_error);
        public void GoBack()
        {
            OnBackPressed();
        }

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

            _nickname = FindViewById<TextView>(Resource.Id.nicknameText);
            _nicInputLayout = FindViewById<TextInputLayout>(Resource.Id.nicknameTextLayout);

            _nickname.Click += (sender, args) =>
            {
                _nicInputLayout.Error = "";
            };

            _nickname.FocusChange += (sender, args) =>
            {
                if (args.HasFocus)
                    _nicInputLayout.Error = "";
            };

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


            _pictureButton = FindViewById<Button>(Resource.Id.addPictures);
            _pictureButton.Click += PictureButtonClick;

            var submitButton = FindViewById<Button>(Resource.Id.createPlace);
            submitButton.Click += async (sender, args) =>
            {
                var imm = (InputMethodManager)this.GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(submitButton.WindowToken, 0);
                if (OnSubmitButtonPress != null)
                    await OnSubmitButtonPress();
            };

            var recycler = FindViewById<RecyclerView>(Resource.Id.yourPictures);
            recycler.SetLayoutManager(new GridLayoutManager(this, 3, (int)Orientation.Vertical, false));
            _pictureAdapter = new PlacePictureAdapter();
            recycler.SetAdapter(_pictureAdapter);

            var backButton = FindViewById<View>(Resource.Id.back_button);
            backButton.Click += (sender, args) => OnBackPressed();
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
                    VariantId = variant.Id;
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
            var bitmap = picture.GetRotated(Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                Resources.GetDimensionPixelSize(Resource.Dimension.image_size));
            _fountainsTypeAdapter.AddPicture(variantId, bitmap);
            if (VariantId.HasValue && VariantId.Value == variantId)
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
                    Address = data.GetStringExtra("address");
                    Latitude = Convert.ToDecimal(data.GetDoubleExtra("latitude", 0));
                    Longitude = Convert.ToDecimal(data.GetDoubleExtra("longitude", 0));
                    var fountainPosition =
                        new LatLng(data.GetDoubleExtra("latitude", 0), data.GetDoubleExtra("longitude", 0));
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(fountainPosition, 17));
                    _text.Visibility = ViewStates.Gone;
                    _cover.Background = null;
                    _map.AddMarker(new MarkerOptions()
                        .SetPosition(fountainPosition)
                        .SetTitle("Selected position"));
                    MapError = "";
                    break;
                }
                case Result.Ok when requestCode == 2 && data != null && (data.Data != null || data.ClipData != null):
                {
                    _pictureButton.Error = null;
                    if (data.Data == null)
                    {
                        var res = data.ClipData;

                        var pictures = new List<(Uri, Bitmap)>(res.ItemCount);

                        for (var i = 0; i < res.ItemCount; i++)
                        {
                            var uri = res.GetItemAt(i).Uri;
                            var stream = ContentResolver.OpenInputStream(uri);
                            var bitmap = stream.GetRotated(
                                Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                                Resources.GetDimensionPixelSize(Resource.Dimension.image_size));
                            pictures.Add((uri, bitmap));
                        }
                        _pictureAdapter.AddImages(pictures);
                    }
                    else
                    {
                        var stream = ContentResolver.OpenInputStream(data.Data);
                        var bitmap = stream.GetRotated(
                            Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                            Resources.GetDimensionPixelSize(Resource.Dimension.image_size));
                        _pictureAdapter.AddImages(new List<(Uri, Bitmap)>{(data.Data, bitmap) });
                    }

                    break;
                }
                case Result.Ok when requestCode == 2 && (data == null || data.Data == null && data.ClipData == null):
                {
                    _pictureButton.Error = null;
                    var stream = ContentResolver.OpenInputStream(_photoURI);
                    var bitmap = stream.GetRotated(
                        Resources.GetDimensionPixelSize(Resource.Dimension.image_size),
                        Resources.GetDimensionPixelSize(Resource.Dimension.image_size));
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