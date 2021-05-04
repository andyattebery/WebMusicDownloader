using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MusicDownloader.Requests;
using MusicDownloader.Responses;
using MusicDownloader.Services;

namespace MusicDownloader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly YoutubeDlService _youtubeDlService;

        public DownloadController(YoutubeDlService youtubeDlService)
        {
            _youtubeDlService = youtubeDlService;
        }

        [HttpPost]
        [Route("songs")]
        public async Task<ActionResult<List<DownloadResponse>>> DownloadCoverSongs(List<SongRequest> requests)
        {
            return await _youtubeDlService.DownloadSongs(requests);
        }
    }
}