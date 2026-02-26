using Google.Apis.Upload;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace VOD_Manager;

internal partial class Program
{
    private static long lastBytesSent = 0;
    private static DateTime lastTime = DateTime.UtcNow;
    private static DateTime startTime = DateTime.UtcNow;

    private static async Task UploadVideo(
        YouTubeService youtube,
        string filePath,
        string title,
        string description,
        DateTime publishAtUtc)
    {
        var video = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = title,
                Description = description,
                CategoryId = "20" // Gaming
            },
            Status = new VideoStatus
            {
                PrivacyStatus = "private",
                PublishAtDateTimeOffset = publishAtUtc
            }
        };

        using var fileStream = new FileStream(filePath, FileMode.Open);

        var request = youtube.Videos.Insert(
            video,
            "snippet,status",
            fileStream,
            "video/*"
        );

        request.ChunkSize = 16 * 1024 * 1024; //16MB Chunk Size

        lastBytesSent = 0;
        lastTime = DateTime.UtcNow;
        startTime = DateTime.UtcNow;

        request.ProgressChanged += progress =>
        {
            if (progress.Status == UploadStatus.Uploading)
            {
                Console.Clear();
                var totalBytes = fileStream.Length;
                var percentage = (double)progress.BytesSent / totalBytes * 100;
                var barWidth = 50;
                var filledWidth = (int)(percentage / 100 * barWidth);
                var progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);
                
                var currentTime = DateTime.UtcNow;
                var timeDiff = (currentTime - lastTime).TotalSeconds;
                var bytesDiff = progress.BytesSent - lastBytesSent;
                var speed = timeDiff > 0 ? bytesDiff / timeDiff : 0;

                lastBytesSent = progress.BytesSent;
                lastTime = currentTime;

                Console.WriteLine($"Upload Progress: [{progressBar}] {percentage:F2}%");
                Console.WriteLine($"Sent: {FormatBytes(progress.BytesSent)} / Total: {FormatBytes(totalBytes)}");
                Console.WriteLine($"Upload Speed: {FormatBytes((long)speed)}/s");
            }
        };
        
        
        
        request.ResponseReceived += response =>
        {
            Console.WriteLine("Upload complete.");
            Console.WriteLine("Video URL: https://youtube.com/watch?v=" + response.Id);
        };

        await request.UploadAsync();
    }

    private static string FormatBytes(long progressBytesSent)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = progressBytesSent;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}