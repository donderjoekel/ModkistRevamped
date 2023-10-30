using System.IO;
using System.Net.Http;
using Modio;
using Modio.Models;
using File = System.IO.File;

namespace TNRD.Modkist.Services;

public class DownloadService
{
    private readonly ModsClient modsClient;
    private readonly HttpClient httpClient;

    public DownloadService(ModsClient modsClient, HttpClient httpClient)
    {
        this.modsClient = modsClient;
        this.httpClient = httpClient;
    }

    public async Task<string?> DownloadMod(Mod mod)
    {
        if (mod.Modfile?.Download == null)
        {
            // TODO: Log error
            return null;
        }

        if (mod.Modfile.Download.BinaryUrl == null)
        {
            // TODO: Log error
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
            // TODO: Log error
            return null;
        }

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception e)
        {
            // TODO: Log error
            return null;
        }

        byte[] buffer;

        try
        {
            buffer = await response.Content.ReadAsByteArrayAsync();
        }
        catch (Exception e)
        {
            // TODO: Log error
            return null;
        }

        string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            Directory.CreateDirectory(tempDirectory);
        }
        catch (Exception e)
        {
            // TODO: Log error
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
            // TODO: Log error
            return null;
        }

        return tempPath;
    }

    public async Task<string?> DownloadMod(uint modId)
    {
        return await DownloadMod(await modsClient[modId].Get());
    }
}
