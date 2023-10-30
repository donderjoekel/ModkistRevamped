using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using TNRD.Modkist.Models;

namespace TNRD.Modkist.Services;

public class ImageCachingService
{
    private const int MAX_CONCURRENT_REQUESTS = 5;
    private static readonly SemaphoreSlim semaphore = new(MAX_CONCURRENT_REQUESTS);
    private readonly Dictionary<string, string> imageCache = new();
    private readonly string cacheDirectory;

    public ImageCachingService()
    {
        cacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Modkist",
            "Cache");

        if (!Directory.Exists(cacheDirectory))
            Directory.CreateDirectory(cacheDirectory);

        string[] paths = Directory.GetFiles(cacheDirectory, "*.json", SearchOption.TopDirectoryOnly);
        foreach (string path in paths)
        {
            CachedImageModel? cachedImageModel =
                JsonConvert.DeserializeObject<CachedImageModel>(File.ReadAllText(path));

            if (cachedImageModel == null)
            {
                // Broken file? Just delete it?
                File.Delete(path);
            }
            else
            {
                imageCache.Add(cachedImageModel.Key, cachedImageModel.Path);
            }
        }
    }

    public Task<string> GetImagePath(Uri uri)
    {
        return GetImagePath(uri.ToString());
    }

    public async Task<string> GetImagePath(string url)
    {
        if (imageCache.TryGetValue(url, out string? path))
            return path;

        return await GetImageFromWeb(url);
    }

    private async Task<string> GetImageFromWeb(string url)
    {
        await semaphore.WaitAsync();

        try
        {
            IHttpClientFactory httpClientFactory = App.GetService<IHttpClientFactory>();
            HttpClient httpClient = httpClientFactory.CreateClient();

            HttpRequestMessage request = new(HttpMethod.Get, url);
            HttpResponseMessage response;

            try
            {
                response = await httpClient.SendAsync(request);
            }
            catch (Exception e)
            {
                return null!;
            }

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                return null!;
            }

            Stream stream = await response.Content.ReadAsStreamAsync();

            Guid newGuid = Guid.NewGuid();

            CachedImageModel image = new()
            {
                Key = url,
                Path = Path.Combine(cacheDirectory, $"{newGuid}.png")
            };

            await File.WriteAllTextAsync(
                Path.Combine(cacheDirectory, $"{newGuid}.json"),
                JsonConvert.SerializeObject(image));

            await using (FileStream fileStream = File.Create(image.Path))
            {
                await stream.CopyToAsync(fileStream);
            }

            imageCache.Add(url, image.Path);

            return image.Path;
        }
        finally
        {
            semaphore.Release();
        }
    }
}
