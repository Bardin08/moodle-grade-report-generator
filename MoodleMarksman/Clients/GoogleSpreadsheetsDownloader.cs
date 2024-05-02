namespace MoodleMarksman.Clients;

/// <summary>
/// Provides functionality for downloading and saving Google Sheets data as CSV files.
/// </summary>
public class GoogleSpreadsheetDownloader
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleSpreadsheetDownloader"/> class.
    /// </summary>
    /// <param name="httpClient">An optional HttpClient instance. If not provided, a new instance will be created.</param>
    public GoogleSpreadsheetDownloader(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient();
    }

    /// <summary>
    /// Downloads a Google Sheet page as a CSV file and saves it to the specified path.
    /// </summary>
    /// <param name="spreadsheetId">The ID of the Google Sheet.</param>
    /// <param name="sheetGid">The GID (sheet ID) of the sheet within the spreadsheet.</param>
    /// <param name="outputFilePath">The path to save the CSV file.</param>
    /// <returns>A Task representing the asynchronous download operation.</returns>
    public async Task<Stream> DownloadSheetAsCsv(
        string spreadsheetId, string sheetGid, string outputFilePath = "output_file.csv")
    {
        var downloadUrl = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=csv&gid={sheetGid}";

        await using var downloadStream = await _httpClient.GetStreamAsync(downloadUrl);
        var memStream = new MemoryStream();

        var buffer = new byte[4096];
        int bytesRead;
        while ((bytesRead = await downloadStream.ReadAsync(buffer)) > 0)
        {
            await memStream.WriteAsync(buffer.AsMemory(0, bytesRead));
        }

        memStream.Seek(0, SeekOrigin.Begin);
        return memStream;
    }
}
