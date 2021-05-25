using ASPCSoundCloud.Data;
using ASPCSoundCloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPCSoundCloud.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SongsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // Return Index page with songs and playlists related to user
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            ViewModel viewModel = new ViewModel();
            viewModel.SongModels = await _context.SongModel
                .Where(x => x.UserId == userId)
                .ToListAsync();
            viewModel.Playlists = await _context.Playlist
                .Where(y => y.UserId == userId)
                .ToListAsync();
            return View(viewModel);
        }

        // create a playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlaylist([Bind("PlaylistId,Name,Description")] Playlist playlist)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            if (ModelState.IsValid)
            {
                playlist.UserId = userId;

                _context.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction("EditPlaylist", "Songs", new { id = playlist.PlaylistId });
            }
            return RedirectToAction(nameof(Index));
        }

        // create a song
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,SongName,SongArtist,SongGenre,SongFile")] SongModel songModel)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            List<string> authorizedExtentions = new List<string> { ".mp3", ".aac", ".wav" };

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string songName = Path.GetFileNameWithoutExtension(songModel.SongFile.FileName);
                string extension = Path.GetExtension(songModel.SongFile.FileName);
                if (!authorizedExtentions.Contains(extension))
                {
                    TempData["error"] = "L'extention du fichier n'est pas valable";
                    return RedirectToAction(nameof(Index));
                }
                songName = songName + DateTime.Now.ToString("yymmssfff") + extension;
                songModel.SongPath = songName;
                string path = Path.Combine(wwwRootPath + "/Songs", songName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await songModel.SongFile.CopyToAsync(fileStream);
                }

                songModel.UserId = userId;

                _context.Add(songModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // The user can manage tje playlist by adding or removing songs
        public async Task<IActionResult> EditPlaylist(int? id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }

            if (!RightUser(playlist.UserId))
            {
                ViewData["message"] = "You can't access this page";
                return View("Cant");
            }

            ViewModel2 viewModel = new ViewModel2();

            List<ToPlaylist> toPlaylist = new List<ToPlaylist>();
            List<SongModel> songAdded = new List<SongModel>();
            toPlaylist = await _context.ToPlaylist
                .Where(x => x.PlaylistId == playlist.PlaylistId)
                .ToListAsync();

            foreach(ToPlaylist toplaylist in toPlaylist)
            {
                SongModel song = await _context.SongModel.FindAsync(toplaylist.SongId);
                songAdded.Add(song);
            }

            viewModel.playlist = playlist;
            viewModel.SongModels = await _context.SongModel
                .Where(x => x.UserId == userId)
                .ToListAsync();
            viewModel.SongsAdded = songAdded;

            return View(viewModel);
        }

        // return playlist page
        public async Task<IActionResult> PlayPlaylist(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .FirstOrDefaultAsync(m => m.PlaylistId == id);
            if (playlist == null)
            {
                return NotFound();
            }

            if (!RightUser(playlist.UserId))
            {
                ViewData["message"] = "You can't access this page";
                return View("Cant");
            }

            ViewModel2 viewModel = new ViewModel2();

            List<ToPlaylist> toPlaylist = new List<ToPlaylist>();
            List<SongModel> songAdded = new List<SongModel>();
            toPlaylist = await _context.ToPlaylist
                .Where(x => x.PlaylistId == playlist.PlaylistId)
                .ToListAsync();

            foreach (ToPlaylist toplaylist in toPlaylist)
            {
                SongModel song = await _context.SongModel.FindAsync(toplaylist.SongId);
                songAdded.Add(song);
            }

            viewModel.playlist = playlist;
            viewModel.SongsAdded = songAdded;

            return View(viewModel);
        }

        // delete a song 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSong(IFormCollection collection)
        {
            var id = int.Parse(collection["SongId"]);
            var songJoin = await _context.ToPlaylist
                .Where(x => x.SongId == id)
                .ToListAsync();

            string wwwRootPath = _hostEnvironment.WebRootPath;
            var songModel = await _context.SongModel.FindAsync(id);
            var fileName = songModel.SongPath;
            string path = Path.Combine(wwwRootPath + "/Songs", fileName);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            foreach (ToPlaylist join in songJoin)
            {
                _context.ToPlaylist.Remove(join);
            }

            _context.SongModel.Remove(songModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete a playlist
        public async Task<IActionResult> DeletePlaylist(IFormCollection collection)
        {
            var id = int.Parse(collection["PlaylistId"]);
            var playlist = await _context.Playlist.FindAsync(id);

            if (!RightUser(playlist.UserId))
            {
                ViewData["message"] = "You can't access this page";
                return View("Cant");
            }

            var songJoin = await _context.ToPlaylist
                .Where(x => x.PlaylistId == id)
                .ToListAsync();

            foreach (ToPlaylist join in songJoin)
            {
                _context.ToPlaylist.Remove(join);
            }

            _context.Playlist.Remove(playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // delete a song from a playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromPlaylist(IFormCollection collection)
        {
            var id = int.Parse(collection["SongId"]);
            var playlistId = int.Parse(collection["PlaylistId"]);
            var songJoin = await _context.ToPlaylist
                .Where(x => x.SongId == id && x.PlaylistId == playlistId)
                .FirstAsync();

            _context.ToPlaylist.Remove(songJoin);

            await _context.SaveChangesAsync();
            return RedirectToAction("EditPlaylist", "Songs", new { id = playlistId });
        }

        // manager songs from a playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSongs(IFormCollection collection)
        {
            
            var id = int.Parse(collection["PlaylistId"]);

            var songJoin = await _context.ToPlaylist
                .Where(x => x.PlaylistId == id)
                .ToListAsync();

            foreach (ToPlaylist join in songJoin)
            {
                _context.ToPlaylist.Remove(join);
            }
            var x = 0;
            
            foreach (var key in collection.Keys)
            {
                if (x > 0 && x < collection.Count - 1)
                {
                    var songId = int.Parse(collection[key.ToString()]);
                    ToPlaylist join = new ToPlaylist { SongId = songId, PlaylistId = id };
                    _context.Add(join);
                }

                x++;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("EditPlaylist", "Songs", new { id = id });
        }

        private bool RightUser(string id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            if (userId == id)
            {
                return true;
            }

            return false;
        }
    }
}
