using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Modio;
using Modio.Models;
using File = System.IO.File;

namespace TNRD.Modkist.Services;

public class DownloadService
{
    private readonly ModsClient modsClient;
    private readonly HttpClient httpClient;
    private readonly ILogger<DownloadService> logger;

    public DownloadService(ModsClient modsClient, HttpClient httpClient, ILogger<DownloadService> logger)
    {
        this.modsClient = modsClient;
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<string?> DownloadMod(Mod mod)
    {
        if (mod.Modfile == null)
        {
            logger.LogWarning("Mod {Id} has no modfile", mod.Id);
            return null;
        }

        if (mod.Modfile?.Download == null)
        {
            logger.LogWarning("Mod {Id} has no download", mod.Id);
            return null;
        }

        if (mod.Modfile.Download.BinaryUrl == null)
        {
            logger.LogWarning("Mod {Id} has no binary url", mod.Id);
            return null;
        }

        return await DownloadAndSaveFile(mod.Modfile.Download.BinaryUrl);
    }

    private async Task<string?> DownloadAndSaveFile(Uri binaryUrl)
    {
        HttpResponseMessage response;

        try
        {
            response = await httpClient.GetAsync(binaryUrl);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unable to get file from {Url}", binaryUrl);
            return null;
        }

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to ensure status code for {Url}", binaryUrl);
            return null;
        }

        byte[] buffer;

        try
        {
            buffer = await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to read content as byte array for {Url}", binaryUrl);
            return null;
        }

        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            Directory.CreateDirectory(tempDirectory);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to create temp directory");
            return null;
        }

        string tempFilename = Path.GetRandomFileName();
        string tempPath = Path.Combine(tempDirectory, tempFilename);

        try
        {
            await File.WriteAllBytesAsync(tempPath, buffer);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to write file to {Path}", tempPath);
            return null;
        }

        return tempPath;
    }

    public async Task<string?> DownloadMod(uint modId)
    {
        return await DownloadMod(await modsClient[modId].Get());
    }
}
