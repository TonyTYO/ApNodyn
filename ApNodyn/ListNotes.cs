using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.FloatingActionButton;
using System.Collections.Generic;

namespace ApNodyn
{
    [Activity(Label = "ListNotes")]
    public class ListNotes : AppCompatActivity
    {
        NotesAdapter notesAdapter;
        RecyclerView recyclerView;
        LinearLayoutManager layoutManager;

        private NoteDatabase database;
        public List<Note> notes = new List<Note>();
        public int menu;
        private FloatingActionButton fab;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Retrieve menu option sent with intent
            menu = Intent.GetIntExtra("menu", 0);
            database = MainActivity.Database;
            LoadData();

            // Create your application here
            SetContentView(Resource.Layout.activity_list_notes);

            // Set click event on fab button
            // Return Id of note added
            fab = FindViewById<FloatingActionButton>(Resource.Id.idfab);
            fab.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(NoteEntry));
                StartActivityForResult(intent, 1);
            };

            recyclerView = FindViewById<RecyclerView>(Resource.Id.notesRV);
            layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);

            notesAdapter = new NotesAdapter(notes);
            recyclerView.SetAdapter(notesAdapter);

            // Specify event handlers for click events
            notesAdapter.ItemClick += OnItemClick;
            notesAdapter.DeleteClick += OnDeleteClick;
            notesAdapter.VisibleChange += OnVisibleChange;
            notesAdapter.HighlightChange += OnHighliteChange;
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Retrieve all the notes from the database, and set them as the
            // data source for the CollectionView.
        }

        protected override void OnResume()
        {
            base.OnResume();

        }

        // Get result from activity
        // requestcode would contain id integer for activity call
        // As we only use one no need to test for it
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                int id = data.GetIntExtra("note", 0);
                if (id > 0)
                {
                    notes.Add(database.GetNote(id));
                    SendUpdate();
                    Toast.MakeText(Application.Context, "Note Id:" + id + " added", ToastLength.Short).Show();
                }
            }
        }

        // Load notes from database based on menu option
        private void LoadData()
        {
            switch (menu)
            {
                case 2:
                    notes = database.GetNotes();
                    break;
                case 3:
                    notes = database.GetActive();
                    break;
                case 4:
                    notes = database.GetVisible();
                    break;
                case 5:
                    notes = database.GetWidget();
                    break;
                default:
                    notes = database.GetNotes();
                    break;
            }
        }

        // Handlers for click events 
        void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this, typeof(NoteEntry));
            intent.PutExtra("Id", notes[position].ID);
            StartActivity(intent);
        }

        void OnDeleteClick(object sender, int position)
        {
            Note note = notes[position];
            int id = note.ID;
            database.DeleteNote(note);
            notes.RemoveAt(position);
            SendUpdate();
            Toast.MakeText(Application.Context, "Note Id:" + id + " deleted", ToastLength.Short).Show();
        }

        void OnVisibleChange(object sender, int position)
        {
            if (notesAdapter.IsBinding) return;
            Note note = notes[position];
            int id = note.ID;
            note.Visible = !note.Visible;
            database.SaveNote(note);
            SendUpdate();
            Toast.MakeText(Application.Context, "OnVisibleChange: " + id + " " + note.Visible, ToastLength.Short).Show();
        }

        void OnHighliteChange(object sender, int position)
        {
            if (notesAdapter.IsBinding) return;
            Note note = notes[position];
            int id = note.ID;
            note.Highlight = !note.Highlight;
            database.SaveNote(note);
            SendUpdate();
            Toast.MakeText(Application.Context, "OnHighlightChange: " + id + " " + note.Highlight, ToastLength.Short).Show();
        }

        // Notify adapter and widget of changes to data
        void SendUpdate()
        {
            notesAdapter.UpdateList(notes);

            Context context = ApplicationContext;
            AppWidgetManager WidgetManager = AppWidgetManager.GetInstance(context);
            ComponentName thisWidget = new ComponentName(context, Java.Lang.Class.FromType(typeof(StickyNote)).Name);
            int[] appWidgetIds = WidgetManager.GetAppWidgetIds(thisWidget);
            WidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.listViewWidget);
        }

    }



}