using MudBlazor;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using Shared.Models;

namespace Client.Services;

public interface IUploadCsvService
{
    Task<MeterReadingUploadResult?> SendPost(IBrowserFile file, ISnackbar snackbar);
}

public class UploadCsvService : IUploadCsvService
{
    private readonly HttpClient _httpClient;

    public UploadCsvService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<MeterReadingUploadResult?> SendPost(IBrowserFile file, ISnackbar snackbar)
    {
        try
        {
            var content = new MultipartFormDataContent();

            // Read the file into memory as a stream
            var streamContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // Add file to the form data
            content.Add(streamContent, "file", file.Name);

            var response = await _httpClient.PostAsync("meter-reading-upload", content);

            if (response.IsSuccessStatusCode)
            {
                snackbar.Add("Upload successful!", Severity.Success);
                return await response.Content.ReadFromJsonAsync<MeterReadingUploadResult>();
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                snackbar.Add("You are not authorized to upload to this resource.", Severity.Error);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                snackbar.Add($"Upload failed: {errorContent}", Severity.Error);
            }
            return null;
        }
        catch (Exception ex)
        {
            snackbar.Add($"An error occurred: {ex.Message}", Severity.Error);
            return null;
        }
    }
}
