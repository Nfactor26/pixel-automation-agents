namespace Pixel.Automation.Agents.Core;

/// <summary>
/// DataFile represents a file and details of the file
/// </summary>
public class DataFile
{
    /// <summary>
    /// Name of the file
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Path of the file
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Version of the file
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Type of the  file
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// File data as byte[]
    /// </summary>
    public byte[] Bytes { get; set; }
}
