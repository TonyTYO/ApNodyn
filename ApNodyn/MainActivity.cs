using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using System.IO;

namespace ApNodyn
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static NoteDatabase database;

        // Create the database connection as a singleton.
        public static NoteDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new NoteDatabase(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return database;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //change main_compat_menu
            MenuInflater.Inflate(Resource.Menu.mainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        // Deal with menu options
        // Menu option number sent with intent
        public override bool OnOptionsItemSelected(IMenuItem item)
        {

            switch (item.ItemId)
            {
                case Resource.Id.menu_main_add:
                case Resource.Id.action_add:
                    Intent intent = new Intent(this, typeof(NoteEntry));
                    intent.PutExtra("menu", 1);
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_main_list:
                case Resource.Id.action_list:
                    intent = new Intent(this, typeof(ListNotes));
                    intent.PutExtra("menu", 2);
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_main_active:
                    intent = new Intent(this, typeof(ListNotes));
                    intent.PutExtra("menu", 3);
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_main_visible:
                    intent = new Intent(this, typeof(ListNotes));
                    intent.PutExtra("menu", 4);
                    StartActivity(intent);
                    break;
                case Resource.Id.menu_main_widget:
                    intent = new Intent(this, typeof(WidgetList));
                    intent.PutExtra("menu", 5);
                    StartActivity(intent);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

    }
}