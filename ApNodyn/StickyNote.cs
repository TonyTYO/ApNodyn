using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace ApNodyn
{
    [BroadcastReceiver(Label = "StickyNote")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetinfo")]
    internal class StickyNote : AppWidgetProvider
    {
        public static string EXTRA_ITEM = "com.example.notesapp.EXTRA_ITEM";
        private string TOAST_ACTION = "com.example.notesapp.TOAST_ACTION";
        private string SCHEDULED_UPDATE = "com.example.notesapp.SCHEDULED_UPDATE";
        private int ALARM_REQUEST_CODE = 999;

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            foreach (int widgetId in appWidgetIds)
            {
                RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.sticky_note);
                string PACKAGE_NAME = context.PackageName;

                // Set remote adaptor
                Intent svcIntent = new Intent(context, typeof(StickyNoteService));
                svcIntent.SetPackage(PACKAGE_NAME);
                svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);
                svcIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(IntentUriType.AndroidAppScheme)));
                remoteViews.SetRemoteAdapter(Resource.Id.listViewWidget, svcIntent);

                // Pending event to open ListNotes on click at top of widget
                Intent listIntent = new Intent(context, typeof(ListNotes));
                listIntent.SetPackage(PACKAGE_NAME);
                listIntent.PutExtra("menu", 3);
                PendingIntent pendingIntent = PendingIntent.GetActivity(context, 0, listIntent, PendingIntentFlags.UpdateCurrent);
                remoteViews.SetOnClickPendingIntent(Resource.Id.widgetTitleLabel, pendingIntent);

                // Pending intent Template to show note extra on click in message
                Intent toastIntent = new Intent(context, typeof(StickyNote));
                toastIntent.SetPackage(PACKAGE_NAME);
                toastIntent.SetAction(TOAST_ACTION);
                toastIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);
                toastIntent.SetData(Android.Net.Uri.Parse(svcIntent.ToUri(IntentUriType.AndroidAppScheme)));
                PendingIntent toastPendingIntent = PendingIntent.GetBroadcast(context, 0, toastIntent, PendingIntentFlags.UpdateCurrent);
                remoteViews.SetPendingIntentTemplate(Resource.Id.listViewWidget, toastPendingIntent);

                appWidgetManager.UpdateAppWidget(widgetId, remoteViews);
            }
        }

        public override void OnEnabled(Context context)
        {
            base.OnEnabled(context);
            ScheduleNextUpdate(context);
        }

        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            base.OnDeleted(context, appWidgetIds);

            // Cancel alarm by setting up a similar pendingintent and then cancelling
            AlarmManager alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            Intent intent = new Intent(context, typeof(StickyNote));
            intent.SetAction(SCHEDULED_UPDATE);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, ALARM_REQUEST_CODE, intent, PendingIntentFlags.NoCreate);
            if (pendingIntent != null && alarmManager != null)
            {
                alarmManager.Cancel(pendingIntent);
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            if (intent.Action == TOAST_ACTION)
            {
                // Show note Extra in Toast
                string description = intent.GetStringExtra(EXTRA_ITEM);
                Toast.MakeText(Application.Context, description, ToastLength.Short).Show();
            }
            else if (intent.Action == SCHEDULED_UPDATE)
            {
                AppWidgetManager manager = AppWidgetManager.GetInstance(context);
                ComponentName thisWidget = new ComponentName(context, Java.Lang.Class.FromType(typeof(StickyNote)).Name);
                int[] ids = manager.GetAppWidgetIds(thisWidget);
                manager.NotifyAppWidgetViewDataChanged(ids, Resource.Id.listViewWidget);
                OnUpdate(context, manager, ids);
            }
        }

        // Schedule update at 1 min past midnight
        private void ScheduleNextUpdate(Context context)
        {
            AlarmManager alarmManager = context.GetSystemService(Context.AlarmService) as AlarmManager;
            Intent intent = new Intent(context, typeof(StickyNote));
            intent.SetAction(SCHEDULED_UPDATE);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(context, ALARM_REQUEST_CODE, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            calendar.Set(Java.Util.CalendarField.HourOfDay, 0);
            calendar.Set(Java.Util.CalendarField.Minute, 5);
            calendar.Set(Java.Util.CalendarField.Second, 0);
            calendar.Set(Java.Util.CalendarField.Millisecond, 0);

            alarmManager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);
        }

    }
}