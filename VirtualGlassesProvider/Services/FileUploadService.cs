using VirtualGlassesProvider.Models;


namespace VirtualGlassesProvider.Services
{
    public static class FileUploadService
    {
        public static UploadedImages? ConvertFormFileToUploadedImageObject(IFormFile file)
        {
            UploadedImages? upload = null;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    upload = new UploadedImages()
                    {
                        Image = memoryStream.ToArray()
                    };
                }
            }
            return upload;
        }
    }
}
