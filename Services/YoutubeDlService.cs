using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MusicDownloader.Requests;
using MusicDownloader.Responses;
using NYoutubeDL;

namespace MusicDownloader.Services
{
    public class YoutubeDlService
    {
        private const int MaxDownloads = 6;
        private string MusicArtistsDirectory => _appSettings.MusicArtistsDirectory;
        private Lazy<string> DownloadDirectory = new Lazy<string>(() => 
            Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MusicDownloader"), "TempDownloads"));
        private readonly AppSettings _appSettings;

        private readonly SemaphoreSlim _batcher = new SemaphoreSlim(MaxDownloads, MaxDownloads);

        public YoutubeDlService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<List<DownloadResponse>> DownloadSongs(IEnumerable<SongRequest> requests)
        {
            var downloadTasks = requests.Select(DownloadSong).ToArray();

            await Task.WhenAll(downloadTasks);

            return downloadTasks.Select(t => t.Result).ToList();
        }

        private async Task<DownloadResponse> DownloadSong(SongRequest request)
        {
            await _batcher.WaitAsync();

            try
            {
                var youtubeDl = new YoutubeDL();

                var standardOutBuilder = new StringBuilder();
                var standardErrorBuilder = new StringBuilder();
                youtubeDl.StandardOutputEvent += (s, output) => standardOutBuilder.Append(output);
                youtubeDl.StandardErrorEvent += (s, error) => standardErrorBuilder.Append(error);

                var artistPath = Path.Combine(request.Artist[0].ToString(), request.Artist);
                var artistAlbumPath = Path.Combine(artistPath, request.Album);
                var artistAlbumFilePath = Path.Combine(artistAlbumPath, $"{request.Title}.m4a");
                var fullFilePath = Path.Combine(MusicArtistsDirectory, artistAlbumFilePath);
                youtubeDl.Options.FilesystemOptions.Output = fullFilePath;

                youtubeDl.Options.VideoFormatOptions.FormatAdvanced = "bestaudio[ext=m4a]";

                var info = await youtubeDl.GetDownloadInfoAsync();
                await youtubeDl.DownloadAsync(request.Url);

                return new DownloadResponse(request.Url, info.Title, standardOutBuilder.ToString(), standardErrorBuilder.ToString());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _batcher.Release();
            }
        }
    }
}