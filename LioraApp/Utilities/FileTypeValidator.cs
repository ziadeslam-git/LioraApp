namespace LioraApp.Utilities;

/// <summary>
/// Validates uploaded files by inspecting their magic bytes (file signatures),
/// not just the file extension or Content-Type header supplied by the client.
/// </summary>
public static class FileTypeValidator
{
    // Known image magic-byte signatures
    private static readonly (string Mime, byte[] Signature)[] _signatures =
    [
        ("image/jpeg", [0xFF, 0xD8, 0xFF]),
        ("image/png",  [0x89, 0x50, 0x4E, 0x47]),
        ("image/webp", [0x52, 0x49, 0x46, 0x46]),  // RIFF header; WEBP follows at offset 8
    ];

    /// <summary>Maximum allowed receipt file size (5 MB).</summary>
    public const long MaxReceiptBytes = 5 * 1024 * 1024;

    /// <summary>
    /// Returns <c>true</c> when the file's actual byte content matches a known
    /// image signature. Sets <paramref name="detectedMimeType"/> on success.
    /// </summary>
    public static bool IsValidImage(IFormFile file, out string detectedMimeType)
    {
        detectedMimeType = string.Empty;

        if (file is null || file.Length == 0)
            return false;

        // Read only the first 8 bytes — avoids loading the entire file into memory.
        // stackalloc keeps the header on the stack (no heap allocation).
        Span<byte> header = stackalloc byte[8];
        using var stream = file.OpenReadStream();
        int bytesRead = stream.Read(header);
        if (bytesRead < 4) return false;

        foreach (var (mime, sig) in _signatures)
        {
            if (header.StartsWith(sig))
            {
                detectedMimeType = mime;
                return true;
            }
        }

        return false;
    }
}
