namespace WebApp.Services
{
    public static class ImageUploadHelper
    {
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

        // Returns (null, null) when no file was provided, (null, error) - invalid,
        // or (path, null) - success.
        public static async Task<(string? path, string? error)> SaveAsync(
            IFormFile? file, string webRootPath, string subfolder)
        {
            if (file == null || file.Length == 0) return (null, null);

            if (file.Length > MaxBytes)
                return (null, "Image must be 5 MB or smaller.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext))
                return (null, "Only image files (jpg, png, gif, webp) are allowed.");

            var dir = Path.Combine(webRootPath, "uploads", subfolder);
            Directory.CreateDirectory(dir);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(dir, fileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            return ($"/uploads/{subfolder}/{fileName}", null);
        }

        // Deletes an image if it was changed by another one
        public static void DeleteIfLocal(string? webPath, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(webPath)) return;
            if (!webPath.StartsWith("/uploads/")) return;

            var relative = webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(webRootPath, relative);
            try { if (File.Exists(fullPath)) File.Delete(fullPath); }
            catch { /* if a file is missing then nothing happens */ }
        }
    }
}
