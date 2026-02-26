using System;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace VOD_Manager
{
    internal partial class Program
    {
        static readonly string[] Scopes = [YouTubeService.Scope.YoutubeUpload];
        const string ApplicationName = "VOD-Manager";
        static async Task Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("Usage:");
                Console.WriteLine("  dotnet run -- <file> <title> <description> <publish-utc>");
                Console.WriteLine("Example:");
                Console.WriteLine("  dotnet run -- video.mp4 \"Title\" \"Description\" 2027-03-01T18:00:00");
                return;
            }
            string filePath = args[0];
            string title = args[1];
            string description = args[2];
            DateTime publishAtUtc = DateTime.SpecifyKind(
                DateTime.Parse(args[3]),
                DateTimeKind.Utc
            );
            var youtube = await Authenticate();
            string videoPath = MergeVideo(filePath);
            await UploadVideo(youtube, videoPath, title, description, publishAtUtc);
            File.Delete(videoPath);
        }
    }
}