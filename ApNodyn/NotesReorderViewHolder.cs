using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using ApNodyn.Helper;
using Google.Android.Material.Card;
using Google.Android.Material.SwitchMaterial;
using System;

namespace ApNodyn
{
    internal class NotesReorderViewHolder : RecyclerView.ViewHolder, IItemTouchHelperViewHolder
    {
        public TextView NoteName;
        public TextView NoteDescription;
        public TextView NoteActivation;
        public SwitchMaterial NoteVisible;
        public SwitchMaterial NoteHighlight;
        public TextView NoteUpdated;
        public ImageView NoteDelete;
        public MaterialCardView ItemCard;

        private View itemView;
        private Color colorPrimary;
        private Color colorMove;

        // Use the constructor that takes in a View
        public NotesReorderViewHolder(View itemView, Action<int> listener, Action<int> del_listener, Action<int> vis_listener, Action<int> col_listener) : base(itemView)
        {
            this.itemView = itemView;
            NoteName = itemView.FindViewById<TextView>(Resource.Id.tvNote);
            NoteDescription = itemView.FindViewById<TextView>(Resource.Id.tvExtra);
            NoteActivation = itemView.FindViewById<TextView>(Resource.Id.tvActivate);
            NoteVisible = itemView.FindViewById<SwitchMaterial>(Resource.Id.smVisibility);
            NoteHighlight = itemView.FindViewById<SwitchMaterial>(Resource.Id.smColour);
            NoteUpdated = itemView.FindViewById<TextView>(Resource.Id.tvDate);
            NoteDelete = itemView.FindViewById<ImageView>(Resource.Id.ivDelete);
            ItemCard = itemView.FindViewById<MaterialCardView>(Resource.Id.cardItem);


            // Set up listeners for switch change, delete click and item click
            NoteVisible.CheckedChange += (sender, e) => vis_listener(base.LayoutPosition);
            NoteHighlight.CheckedChange += (sender, e) => col_listener(base.LayoutPosition);
            NoteDelete.Click += (sender, e) => del_listener(base.LayoutPosition);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);

            // Get colours from color.xml
            colorPrimary = new Android.Graphics.Color(ContextCompat.GetColor(Application.Context, Resource.Color.colorPrimary));
            colorMove = new Android.Graphics.Color(ContextCompat.GetColor(Application.Context, Resource.Color.colorMove));
        }

        public void onItemClear()
        {
            itemView.SetBackgroundColor(colorPrimary);
        }

        public void onItemSelected()
        {
            if (itemView.Pressed)
            {
                itemView.SetBackgroundColor(colorMove);
            }
        }
    }
}