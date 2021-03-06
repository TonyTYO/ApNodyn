namespace ApNodyn.Helper
{
    internal interface IItemTouchHelperAdapter
    {
        /**
         * Called when an item has been dragged far enough to trigger a move. This is called every time
         * an item is shifted, and <strong>not</strong> at the end of a "drop" event.<br/>
         * <br/>
         * Implementations should call {@link RecyclerView.Adapter#notifyItemMoved(int, int)} after
         * adjusting the underlying data to reflect this move.
         *
         * @param fromPosition The start position of the moved item.
         * @param toPosition   Then resolved position of the moved item.
         * @return True if the item was moved to the new adapter position.
         *
         * @see RecyclerView#getAdapterPositionFor(RecyclerView.ViewHolder)
         * @see RecyclerView.ViewHolder#getAdapterPosition()
         */
        bool onItemMove(int fromPosition, int toPosition);

        /**
         * Called when an item drag has finished
         * <br/>
         * Implementations should call {@link RecyclerView.Adapter#notifyItemMoved(int, int)} after
         * adjusting the underlying data to reflect this move.
         *
         * @param fromPosition The start position of the moved item.
         * @param toPosition   Then end position of the moved item.
         * @return True if the item was moved to the new adapter position.
         *
         * @see RecyclerView#getAdapterPositionFor(RecyclerView.ViewHolder)
         * @see RecyclerView.ViewHolder#getAdapterPosition()
         */
        bool onItemMoveEnd(int fromPosition, int toPosition);


        /**
         * Called when an item has been dismissed by a swipe.<br/>
         * <br/>
         * Implementations should call {@link RecyclerView.Adapter#notifyItemRemoved(int)} after
         * adjusting the underlying data to reflect this removal.
         *
         * @param position The position of the item dismissed.
         *
         * @see RecyclerView#getAdapterPositionFor(RecyclerView.ViewHolder)
         * @see RecyclerView.ViewHolder#getAdapterPosition()
         */
        void onItemDismiss(int position);
    }
}