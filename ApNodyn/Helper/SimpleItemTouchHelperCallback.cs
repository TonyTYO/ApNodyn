using AndroidX.RecyclerView.Widget;

namespace ApNodyn.Helper
{
    internal class SimpleItemTouchHelperCallback : ItemTouchHelper.Callback
    {
        public static float ALPHA_FULL = 1.0f;

        private IItemTouchHelperAdapter mAdapter;
        // start and end positions of drag
        private int fromPosition = -1;
        private int toPosition = -1;

        public SimpleItemTouchHelperCallback(IItemTouchHelperAdapter adapter)
        {
            mAdapter = adapter;
        }

        public override int GetMovementFlags(RecyclerView recyclerView, RecyclerView.ViewHolder vholder)
        {
            int dragFlags = ItemTouchHelper.Up | ItemTouchHelper.Down;
            int swipeFlags = ItemTouchHelper.Start | ItemTouchHelper.End;
            return MakeMovementFlags(dragFlags, swipeFlags);

        }

        public override bool OnMove(RecyclerView recyclerView, RecyclerView.ViewHolder source, RecyclerView.ViewHolder target)
        {
            if (source.ItemViewType != target.ItemViewType)
            {
                return false;
            }
            // Set start drag and end drag and notify the adapter of the move
            if (fromPosition == -1) { fromPosition = source.BindingAdapterPosition; }
            toPosition = target.BindingAdapterPosition;
            mAdapter.onItemMove(source.BindingAdapterPosition, target.BindingAdapterPosition);
            return true;
        }

        public override void OnSwiped(RecyclerView.ViewHolder vholder, int i)
        {
            mAdapter.onItemDismiss(vholder.BindingAdapterPosition);
        }

        public override void OnSelectedChanged(RecyclerView.ViewHolder vholder, int actionState)
        {
            // We only want the active item to change
            if (actionState != ItemTouchHelper.ActionStateIdle)
            {
                if (vholder is IItemTouchHelperViewHolder)
                {
                    // Let the view holder know that this item is being moved or dragged
                    IItemTouchHelperViewHolder itemViewHolder = (IItemTouchHelperViewHolder)vholder;
                    itemViewHolder.onItemSelected();
                }
            }
            base.OnSelectedChanged(vholder, actionState);
        }

        public override void ClearView(RecyclerView recyclerView, RecyclerView.ViewHolder vholder)
        {
            base.ClearView(recyclerView, vholder);

            vholder.ItemView.Alpha = ALPHA_FULL;

            if (vholder is IItemTouchHelperViewHolder)
            {
                // Tell the view holder it's time to restore the idle state
                IItemTouchHelperViewHolder itemViewHolder = (IItemTouchHelperViewHolder)vholder;
                itemViewHolder.onItemClear();
            }
            // Notify end of move and reset drag position holders
            if (fromPosition != -1 && toPosition != -1 && fromPosition != toPosition)
            {
                mAdapter.onItemMoveEnd(fromPosition, toPosition);
            }
            fromPosition = toPosition = -1;
        }

    }
}