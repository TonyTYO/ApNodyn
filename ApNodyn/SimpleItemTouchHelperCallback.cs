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
    internal class SimpleItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        private readonly ITemTouchHelperAdapter _mAdapter;

        public SimpleItemTouchHelperCallback(ITemTouchHelperAdapter adapter)
        {
            _mAdapter = adapter;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder)
        {
            const int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            const int swipeFlags = ItemTouchHelper.ActionStateIdle;
            return MakeMovementFlags(dragFlags, swipeFlags);
        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder viewHolder, RecyclerView.ViewHolder target)
        {
            if (viewHolder.ItemViewType != target.ItemViewType)
            {
                return false;
            }

            // Notify the adapter of the move
            _mAdapter.OnItemMove(viewHolder.BindingAdapterPosition, target.BindingAdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder viewHolder, int direction)
        {
            // Notify the adapter of the dismissal
            _mAdapter.OnItemDismiss(viewHolder.BindingAdapterPosition);
        }
    }
}