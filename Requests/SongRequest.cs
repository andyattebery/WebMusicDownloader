using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicDownloader.Requests
{
    public class SongRequest
    {
        public SongRequest(string url, string artist, string album, string title)
        {
            Url = url;
            Artist = artist;
            Album = album;
            Title = title;
        }

        public string Url { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Title { get; set; }

        public string Validate()
        {
            var errors = new List<string>();

            if (!Uri.IsWellFormedUriString(Url, UriKind.Absolute))
            {
                errors.Add($"{nameof(Url)} is not valid.");
            }
            if (string.IsNullOrEmpty(Artist))
            {
                errors.Add($"{nameof(Artist)} is required.");
            }
            if (string.IsNullOrEmpty(Album))
            {
                errors.Add($"{nameof(Album)} is required.");
            }
            if (string.IsNullOrEmpty(Title))
            {
                errors.Add($"{nameof(Title)} is required.");
            }
            
            return errors.Any() ? string.Join("; ", errors) : null;
        }
    }
}