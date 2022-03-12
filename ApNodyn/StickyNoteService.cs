using Android.App;
using Android.Content;
using Android.Widget;

namespace ApNodyn
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    internal class StickyNoteService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            //    int appWidgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);

            StickyNoteListProvider lp = new StickyNoteListProvider(this.ApplicationContext);

            return lp;
        }
    }
}