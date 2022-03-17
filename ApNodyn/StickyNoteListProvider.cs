using Android.App;
using Android.Graphics;
using Android.Content;
using Android.Widget;
using AndroidX.Core.Content;
using System.Collections.Generic;


namespace ApNodyn
{
    internal class StickyNoteListProvider : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private List<Note> listNotes = new List<Note>();
        private Context context;
        private Color colorHighlight;
        private Color colorNormal;


        public StickyNoteListProvider(Context contextNew)

        {
            context = contextNew;
            colorHighlight = new Android.Graphics.Color(ContextCompat.GetColor(Application.Context, Resource.Color.red));
            colorNormal = new Android.Graphics.Color(ContextCompat.GetColor(Application.Context, Resource.Color.black));
        }

        public int Count { get { return listNotes.Count; } }

        public long GetItemId(int position)
        {
            return listNotes[position].ID;
        }

        public RemoteViews GetViewAt(int position)
        {
            RemoteViews remoteView = new RemoteViews(context.PackageName, Resource.Layout.sticky_note_list_item);
            if (listNotes[position].Highlight)
            {
                remoteView.SetTextColor(Resource.Id.widgetItemTaskNameLabel, colorHighlight);

            }
            else
            {
                remoteView.SetTextColor(Resource.Id.widgetItemTaskNameLabel, colorNormal);
            }
            remoteView.SetTextViewText(Resource.Id.widgetItemTaskNameLabel, listNotes[position].Text);

            // Fill in Pending event on click
            Intent fillInIntent = new Intent(context, typeof(Intent));
            fillInIntent.PutExtra(StickyNote.EXTRA_ITEM, listNotes[position].Extra);
            remoteView.SetOnClickFillInIntent(Resource.Id.widgetItemContainer, fillInIntent);

            return remoteView;

        }

        public RemoteViews LoadingView { get { return null; } }

        public int ViewTypeCount { get { return 1; } }

        public bool HasStableIds { get { return true; } }

        public void OnCreate()
        {
            // throw new NotImplementedException();
        }

        public void OnDataSetChanged()
        {
            // throw new NotImplementedException();
            listNotes = MainActivity.Database.GetWidget();
        }

        public void OnDestroy()
        {
            // throw new NotImplementedException();
        }

    }
}