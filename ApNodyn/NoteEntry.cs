using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.SwitchMaterial;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ApNodyn
{

    [Activity(Label = "NoteEntry")]
    public class NoteEntry : AppCompatActivity
    {
        protected EditText etNoteName = null;
        protected EditText etNoteDescription = null;
        protected EditText etNoteActivate = null;
        protected DatePicker dpNoteActivation = null;
        protected SwitchMaterial swNoteVisible = null;
        protected Button btnSave = null;
        protected NoteDatabase database;
        protected int noteId;
        protected int menu;
        Regex comp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_note_entry);

            etNoteName = FindViewById<EditText>(Resource.Id.tieEditNoteName);
            etNoteDescription = FindViewById<EditText>(Resource.Id.tieEditNoteDesc);
            etNoteActivate = FindViewById<EditText>(Resource.Id.tieNoteActivation);
            dpNoteActivation = FindViewById<DatePicker>(Resource.Id.dpActivate);
            swNoteVisible = FindViewById<SwitchMaterial>(Resource.Id.smVisibility);
            // Set button click event
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += (sender, e) => { Save(); };
            // Set datepicker to today
            dpNoteActivation.DateTime = DateTime.Now;

            database = MainActivity.Database;
            menu = Intent.GetIntExtra("menu", 0);
            noteId = Intent.GetIntExtra("Id", 0);

            // If noteId then fill fields and edit
            if (noteId > 0)
            {
                Note note = database.GetNote(noteId);
                etNoteName.Text = note.Text;
                etNoteDescription.Text = note.Extra;
                dpNoteActivation.DateTime = note.Activate;
                etNoteActivate.Text = note.Activate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                swNoteVisible.Checked = note.Visible;
            }

            // Set Datechanged event on datepicker to show date in Activation TextView
            dpNoteActivation.DateChanged += (sender, e) =>
            {
                etNoteActivate.Text = dpNoteActivation.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                Toast.MakeText(Application.Context, "Note Id:" + dpNoteActivation.DateTime, ToastLength.Short).Show();
            };
            // Compile regex for valid date format
            string pattern = "(\\d|\\d{2})-(\\d|\\d{2})-(\\d{2}|\\d{4})";
            comp = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled);
            // If focus leaves TextView and date format valid
            // Try to parse date and if valid set datepicker to match
            etNoteActivate.FocusChange += (sender, e) =>
            {
                Match match = comp.Match(etNoteActivate.Text);
                if (match.Success)
                {
                    string newdate = FormatDate(etNoteActivate.Text);
                    etNoteActivate.Text = newdate;
                    DateTime returnValue;
                    bool flag = DateTime.TryParseExact(etNoteActivate.Text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out returnValue);
                    if (flag)
                        dpNoteActivation.DateTime = returnValue;
                }
            };
        }

        protected void Save()
        {
            Note note = new Note();
            note.ID = noteId;
            note.Text = etNoteName.Text;
            note.Extra = etNoteDescription.Text;
            Match match = comp.Match(etNoteActivate.Text);
            // If date in datepicker different to date in Activation field
            // Use TextView if valid else use datepicker
            if (match.Success && dpNoteActivation.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture) != etNoteActivate.Text)
            {
                note.Activate = DateTime.Parse(etNoteActivate.Text);
            }
            else
            {
                note.Activate = dpNoteActivation.DateTime;
            }
            note.Visible = swNoteVisible.Checked;
            // Set last changed date
            note.Date = DateTime.UtcNow;
            note.Position = 1000;
            if (!string.IsNullOrWhiteSpace(note.Text))
            {
                database.SaveNote(note);
            }
            // If from MainActivity menu send data changed notification to app widget
            // Done automatically if in list view
            if (menu == 1)
            {
                Context context = ApplicationContext;
                AppWidgetManager WidgetManager = AppWidgetManager.GetInstance(context);
                ComponentName thisWidget = new ComponentName(context, Java.Lang.Class.FromType(typeof(StickyNote)).Name);
                int[] appWidgetIds = WidgetManager.GetAppWidgetIds(thisWidget);
                WidgetManager.NotifyAppWidgetViewDataChanged(appWidgetIds, Resource.Id.listViewWidget);
            }

            // Navigate backwards
            // Return note Id with intent
            Intent resultIntent = new Intent();
            resultIntent.PutExtra("note", note.ID);
            SetResult(Result.Ok, resultIntent);

            Finish();
        }

        // Format date string to be dd/mm/yyyy
        private string FormatDate(string date)
        {
            string newdate = "";
            if (!string.IsNullOrWhiteSpace(date))
            {
                string[] result = date.Split('-', '/', '\\', ' ');
                int no = 1;
                foreach (string num in result)
                {
                    if (!string.IsNullOrWhiteSpace(newdate)) newdate += '-';
                    if (no == 3)
                    {
                        newdate += PadString(num, 4, "20");
                    }
                    else
                    {
                        newdate += PadString(num, 2, "0");
                    }
                    no += 1;
                }
            }
            return newdate;
        }

        // Pad with padding to required length
        private string PadString(string str, int len, string padding)
        {
            while (str.Length < len)
            {
                str = padding + str;
            }
            return str;
        }
    }



}