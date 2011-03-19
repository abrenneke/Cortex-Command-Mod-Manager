using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CortexCommandModManager.ModsDatabase
{
    public class ModDatabaseMod
    {
        public int ModId { get; set; }

        public string Title { get; set; }

        public string ShortDescription { get; set; }

        public string LongDescription { get; set; }

        public double AverageRating { get; set; }

        public int NumRatings { get; set; }

        public int UserId { get; set; }

        public string Username { get; set; }

        public int CommentCount { get; set; }

        public int DownloadCount { get; set; }

        public string Screenshot { get; set; }

        public string DownloadLink { get; set; }
    }
}
