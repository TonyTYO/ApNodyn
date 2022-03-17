using Android.Views;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ApNodyn
{
    internal class NotesAdapter : RecyclerView.Adapter
    {
        public List<Note> notes = new List<Note>();

        // Set eventhandlers for Clicks on entry, Delete icon, Visibility Switch
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> DeleteClick;
        public event EventHandler<int> VisibleChange;
        public event EventHandler<int> HighlightChange;

        // Flag set to true while binding is occuring
        public bool IsBinding = false;

        public NotesAdapter(List<Note> ndata)
        {
            notes.Clear();
            notes.AddRange(ndata);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Inflate the layout file
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.note_rv_item, parent, false);

            // Return the view holder (event handlers specified in parameters)
            return new NotesReorderViewHolder(itemView, OnClick, OnDelete, OnVisible, OnHighlight);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NotesReorderViewHolder vholder = holder as NotesReorderViewHolder;

            if (vholder == null)
            {
                return;
            }
            IsBinding = true;
            Note note = notes[position];

            vholder.NoteName.Text = note.Text;
            vholder.NoteDescription.Text = note.Extra;
            vholder.NoteActivation.Text = note.Activate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture); ;
            vholder.NoteVisible.Checked = note.Visible;
            vholder.NoteHighlight.Checked = note.Highlight;
            vholder.NoteUpdated.Text = note.Date.ToString("dd'-'MM'-'yyyy' 'HH':'mm':'ss", CultureInfo.InvariantCulture);
            IsBinding = false;
        }

        public override int ItemCount
        {
            get { return notes.Count; }
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

        void OnVisible(int position)
        {
            if (VisibleChange != null)
                VisibleChange(this, position);
        }

        void OnHighlight(int position)
        {
            if (HighlightChange != null)
                HighlightChange(this, position);
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

    }
}