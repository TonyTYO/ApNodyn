using Android.Views;
using AndroidX.RecyclerView.Widget;
using ApNodyn.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using static Android.Views.View;

namespace ApNodyn
{
    internal class NotesReorderAdapter : RecyclerView.Adapter, IItemTouchHelperAdapter, IOnLongClickListener
    {

        private readonly IOnStartDragListener mDragStartListener;
        private NotesReorderViewHolder reOrderViewHolder;

        public List<Note> notes = new List<Note>();

        // Set eventhandlers for Clicks on entry, Delete icon, Visibility Switch Change, Item moved.
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> DeleteClick;
        public event EventHandler<int> SwitchChange;
        public event EventHandler<ItemMoveEventArgs> PosChange;
        public event EventHandler<ItemMoveEventArgs> MoveEnd;

        // Flag set to true while binding is occuring
        public bool IsBinding = false;

        public NotesReorderAdapter(List<Note> ndata, IOnStartDragListener dragStartListener)
        {
            notes.Clear();
            notes.AddRange(ndata);
            mDragStartListener = dragStartListener;
        }

        public override int ItemCount => notes.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vholder = holder as NotesReorderViewHolder;
            if (vholder == null) return;

            reOrderViewHolder = vholder;
            vholder.ItemCard.SetOnLongClickListener(this);

            IsBinding = true;
            Note note = notes[position];

            vholder.NoteName.Text = note.Text;
            vholder.NoteDescription.Text = note.Extra;
            vholder.NoteActivation.Text = note.Activate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture); ;
            vholder.NoteVisible.Checked = note.Visible;
            vholder.NoteUpdated.Text = note.Date.ToString("dd'-'MM'-'yyyy' 'HH':'mm':'ss", CultureInfo.InvariantCulture);
            IsBinding = false;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.note_rv_item, parent, false);
            return new NotesReorderViewHolder(itemView, OnClick, OnDelete, OnSwitch);
        }

        public bool OnLongClick(View v)
        {
            mDragStartListener.onStartDrag(reOrderViewHolder);
            return true;
        }

        // Handlers for Click events
        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        void OnDelete(int position)
        {
            if (DeleteClick != null)
                DeleteClick(this, position);
        }

        void OnSwitch(int position)
        {
            if (SwitchChange != null)
                SwitchChange(this, position);
        }

        // Handlers for Move events
        public bool onItemMove(int fromPosition, int toPosition)
        {
            PosChange(this, new ItemMoveEventArgs(fromPosition, toPosition));
            return true;
        }

        public bool onItemMoveEnd(int fromPosition, int toPosition)
        {
            MoveEnd(this, new ItemMoveEventArgs(fromPosition, toPosition));
            return true;
        }

        // Handler for Swipe delete
        public void onItemDismiss(int position)
        {
            OnDelete(position);
        }

        // Update notes and notify of updates
        public void UpdateList(List<Note> newlist)
        {
            // Instead of adding new items straight to the main list, create a second list
            List<Note> newNotes = new List<Note>();
            newNotes.AddRange(newlist);

            // Set detectMoves to true for smoother animations
            DiffUtil.DiffResult result = DiffUtil.CalculateDiff(new NotesCallback(notes, newNotes), true);

            // Overwrite the old data
            notes.Clear();
            notes.AddRange(newNotes);

            // Despatch the updates to RecyclerAdapter
            result.DispatchUpdatesTo(this);
        }

        public void PrintNotes(string text, List<Note> rhestr)
        {
            int pos = 1;
            foreach (Note note in rhestr)
            {
                Console.WriteLine(text + " => " + pos + " : " + note.Text + " : " + note.Position);
                pos++;
            }
        }
    }


    public class ItemMoveEventArgs : EventArgs
    {
        // Structure to hold positions for item moved event
        public int fromPosition;
        public int toPosition;

        public ItemMoveEventArgs(int fromPos, int toPos)
        {
            fromPosition = fromPos;
            toPosition = toPos;
        }
    }
}