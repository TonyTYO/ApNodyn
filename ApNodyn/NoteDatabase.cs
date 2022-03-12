using SQLite;
using System.Collections.Generic;

namespace ApNodyn
{
    public class NoteDatabase
    {
        readonly SQLiteConnection database;

        public NoteDatabase(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<Note>();
        }

        public List<Note> GetNotes()
        {
            //Get all notes.
            return database.Table<Note>().ToList();
        }


        public List<Note> GetVisible()
        {
            //Get all visible notes.
            return database.Table<Note>()
                            .Where(i => i.Visible == true)
                            .ToList();
        }

        public List<Note> GetActive()
        {
            //Get all active notes.
            return database.Table<Note>()
                            .Where(i => i.Activate < System.DateTime.Now)
                            .ToList();
        }

        public List<Note> GetWidget()
        {
            //Get all widget notes odered by position then activation date - oldest first
            return database.Table<Note>()
                            .Where(i => i.Activate < System.DateTime.Now && i.Visible == true)
                            .OrderBy(i => i.Position)
                            .ThenBy(i => i.Activate)
                            .ToList();
        }

        public Note GetNote(int id)
        {
            // Get a specific note.
            return database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefault();
        }

        public int SaveNote(Note note)
        {
            if (note.ID != 0)
            {
                // Update an existing note.
                return database.Update(note);
            }
            else
            {
                // Save a new note.
                return database.Insert(note);
            }
        }

        public int DeleteNote(Note note)
        {
            // Delete a note.
            return database.Delete(note);
        }
    }
}