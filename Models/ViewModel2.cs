using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCSoundCloud.Models
{
    public class ViewModel2
    {
        public IEnumerable<SongModel> SongModels { get; set; }
        public IEnumerable<SongModel> SongsAdded { get; set; }
        public Playlist playlist { get; set; }
    }
}
