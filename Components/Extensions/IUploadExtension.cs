using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace dotNet_base.Components.Extensions
{
    public interface IUploadExtension
    {
        public IWebHostEnvironment Environment { get; }

        public async Task<string> Upload(IFormFile file, string dir, string name)
        {
            try {
                if (!dir.EndsWith("/")) {
                    dir += "/";
                }

                var absoluteDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", dir);
                if (!Directory.Exists(absoluteDir)) {
                    Directory.CreateDirectory(absoluteDir);
                }

                var fileName = name + Path.GetExtension(file.FileName);
                var filePath = dir + fileName;
                var absolutePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

                await using var stream = File.Create(absolutePath);
                await file.CopyToAsync(stream);

                return "/" + filePath;
            }
            catch (Exception e) {
                await Console.Out.WriteAsync(e.Message);
                return null;
            }
        }

        public async Task DeleteFile(string path)
        {
            if (path == null) {
                return;
            }

            try {
                if (path.StartsWith("/")) {
                    path = path.Substring(1);
                }

                File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path));
            }
            catch (Exception e) {
                await Console.Error.WriteLineAsync(e.Message);
            }
        }
    }
}