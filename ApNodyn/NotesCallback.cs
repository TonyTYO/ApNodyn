using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;

namespace ApNodyn
{
    internal class NotesCallback : DiffUtil.Callback
    {
        // Compares two lists and returns differences
        private List<Note> oldList;
        private List<Note> newList;

        public NotesCallback(List<Note> oldList, List<Note> newList)
        {
            this.oldList = oldList;
            this.newList = newList;
        }

        public override int OldListSize => oldList.Count;

        public override int NewListSize => newList.Count;

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
        {
            return oldList[oldItemPosition].ID == newList[newItemPosition].ID;
        }

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            Note oldNote = oldList[oldItemPosition];
            Note newNote = newList[newItemPosition];

            return (oldNote.ID == newNote.ID) && (oldNote.Text == newNote.Text) && (oldNote.Extra == newNote.Extra)
                && (oldNote.Activate == newNote.Activate) && (oldNote.Visible == newNote.Visible)
                  && (oldNote.Date == newNote.Date);
        }

        public object getChangePayload(int oldItemPosition, int newItemPosition)
        {
            // Implement method if you're going to use ItemAnimator
            return base.GetChangePayload(oldItemPosition, newItemPosition);
        }
    }
}