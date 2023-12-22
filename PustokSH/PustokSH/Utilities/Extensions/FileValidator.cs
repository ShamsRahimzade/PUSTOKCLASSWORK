using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokSH.Model;

namespace PustokSH.Utilities
{
    public static class FileValidator
    {
        public static bool ValidateType(this IFormFile file, string type)
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }
        public static bool ValiDateSize(this IFormFile file, int kb)
        {
            if (file.Length < kb * 1024*1024)
            {
                return true;
            }
            return false;
        }

        public static async Task<string> CreateFileAsync(this IFormFile file, string root, params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string path = PathCombine(fileName, root, folders);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        public static string PathCombine(string fileName, string root, params string[] folders)
        {
            string path = root;

            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }

            return Path.Combine(path, fileName);
        }
        public static void DeleteFile(this string fileName, string root, params string[] folders)
        {
            string path = PathCombine(fileName, root, folders);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
