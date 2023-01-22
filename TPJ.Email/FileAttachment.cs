namespace TPJ.Email;

public class FileAttachment
{
    public string FilePath { get; set; }

    public byte[] FileBytes { get; set; }

    public string FileName { get; set; }

    public string MimeType { get; set; }


    public FileAttachment()
    {
    }

    public FileAttachment(string filePath, string fileName, string mimeType)
    {
        FilePath = filePath;
        FileName = fileName;
        MimeType = mimeType;
    }

    public FileAttachment(byte[] fileBytes, string fileName, string mimeType)
    {
        FileBytes = fileBytes;
        FileName = fileName;
        MimeType = mimeType;
    }
}
