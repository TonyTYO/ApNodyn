using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using ApNodyn.Helper;
using Google.Android.Material.FloatingActionButton;
using System.Collections.Generic;

namespace ApNodyn
{
    [Activity(Label = "WidgetList")]
    public class WidgetList : AppCompatActivity, IOnStartDragListener
    {
        NotesReorderAdapter notesAdapter;
        RecyclerView recyclerView;
        LinearLayoutManager layoutManager;
        private ItemTouchHelper mItemTouchHelper;

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

            notesAdapter = new NotesReorderAdapter(notes, this);
            recyclerView.SetAdapter(notesAdapter);

            ItemTouchHelper.Callback callback = new SimpleItemTouchHelperCallback(notesAdapter);
            mItemTouchHelper = new ItemTouchHelper(callback);
            mItemTouchHelper.AttachToRecyclerView(recyclerView);

            // Specify event handlers for click events
            notesAdapter.ItemClick += OnItemClick;
            notesAdapter.DeleteClick += OnDeleteClick;
            notesAdapter.SwitchChange += OnSwitchChange;
            notesAdapter.PosChange += OnMove;
            notesAdapter.MoveEnd += OnMoveEnd;
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
            notes = database.GetWidget();
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

        void OnSwitchChange(object sender, int position)
        {
            if (notesAdapter.IsBinding) return;
            Note note = notes[position];
            int id = note.ID;
            note.Visible = !note.Visible;
            if (!note.Visible) { note.Position = 1000; }
            database.SaveNote(note);
            SendUpdate();
            Toast.MakeText(Application.Context, "OnSwitchChange: " + id + " " + note.Visible, ToastLength.Short).Show();
        }

        void OnMove(object sender, ItemMoveEventArgs args)
        {
            int fromPos = args.fromPosition;
            int toPos = args.toPosition;
            Note tempNote = notes[fromPos];

            notes.RemoveAt(fromPos);
            notes.Insert(toPos, tempNote);
            SendUpdate(widget: false);
        }
        // On move end save positions of notes to database and notify widget
        void OnMoveEnd(object sender, ItemMoveEventArgs args)
        {
            Note note = new Note();
            for (var i = 0; i < notes.Count; i++)
            {
                note = notes[i].Copy();
                note.Position = i + 1;
                database.SaveNote(note);
            }
            SendUpdate();
            Toast.MakeText(Application.Context, "OnMoveEnd: " + args.fromPosition + ">>" + args.toPosition, ToastLength.Short).Show();
        }

        // Notify adapter and widget of changes to data
        void SendUpdate(bool widget = true)
        {
            notesAdapter.UpdateList(notes);

            if (widget)
            {
                Context context = ApplicationContext;
                AppWidgetManager WidgetManager = AppWidgetManager.GetInstance(context);
                ComponentName thisWidget = new ComponentName(context, Java.Lang.Class.FromType(typeof(StickyNote)).Name);
                int[] appWidgetIds = WidgetManager.GetAppWidgetIds(thisWidget);
                WidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.listViewWidget);
            }
        }

        public void onStartDrag(RecyclerView.ViewHolder viewHolder)
        {
            mItemTouchHelper.StartDrag(viewHolder);
        }
    }

}