using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPCSoundCloud.Models
{
    public class ViewModel
    {
        public IEnumerable<SongModel> SongModels { get; set; }
        public IEnumerable<Playlist> Playlists { get; set; }
    }
}
