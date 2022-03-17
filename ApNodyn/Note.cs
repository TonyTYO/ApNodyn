using SQLite;
using System;

namespace ApNodyn
{
    public class Note
    {
        /**
         * Notes Table structure
         * 
         * ID           int Primary Key generated automatically
         * Text         string Short text of note
         * Extra        string Extra Information about note
         * Activate     DateTime Date to activate
         * Visible      bool true - show note false - don't show note
         * Highlight    bool true - show in red
         * Position     int Position in list on Sticky note Default impossible value 1000
         * Date         DateTime Date of last change automatically entered
         *
         */

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Text { get; set; }
        public string Extra { get; set; }
        public DateTime Activate { get; set; }
        public bool Visible { get; set; }
        public bool Highlight { get; set; } = false;
        public int Position { get; set; } = 1000;
        public DateTime Date { get; set; }

        // Return shallow copy of note
        public Note Copy()
        {
            return (Note)this.MemberwiseClone();
        }
    }
}