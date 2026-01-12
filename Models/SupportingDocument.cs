using System;

namespace CMCS.Models
{
    public class SupportingDocument
    {
        public int DocumentId { get; set; }
        public int ClaimId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSizeKB { get; set; }
        public DateTime UploadedAt { get; set; }
        public byte[] EncryptedContent { get; set; }
    }
}