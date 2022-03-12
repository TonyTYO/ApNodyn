using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ApNodyn
{
    internal interface IOnStartDragListener
    {
        void OnStartDrag(RecyclerView.ViewHolder viewHolder);
    }
}