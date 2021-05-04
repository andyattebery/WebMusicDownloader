namespace MusicDownloader.Responses
{
    public class DownloadResponse
    {
        public DownloadResponse(string url, string title, string output, string errorOutput)
        {
            Url = url;
            Title = title;
            Output = output;
            ErrorOutput = errorOutput;
        }

        public string Url {get; set;}
        public string Title {get; set;}
        public string Output {get; set;}
        public string ErrorOutput {get; set;}
    }
}