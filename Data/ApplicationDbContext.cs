using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using ASPCSoundCloud.Models;

namespace ASPCSoundCloud.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ASPCSoundCloud.Models.SongModel> SongModel { get; set; }
        public DbSet<ASPCSoundCloud.Models.Playlist> Playlist { get; set; }
        public DbSet<ASPCSoundCloud.Models.ToPlaylist> ToPlaylist { get; set; }
    }
}
