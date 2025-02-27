﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace UserDictionaryReactApp.Helpers
{
    public class FileHelper
    {
        private readonly IWebHostEnvironment _env;
        public FileHelper(IWebHostEnvironment env)
        {
            _env = env;

            // Sometimes WebRootPath may be null, set it to wwwroot
            if (string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                env.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            if (!Directory.Exists(UploadsPath))
            {
                Directory.CreateDirectory(UploadsPath);
            }
        }

        public string UploadsPath => Path.Combine(_env.WebRootPath, "Uploads");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>A file path relative to the /wwwroot/Uploads</returns>
        public string GetFilePath(string fileName)
        {
            return Path.Combine(UploadsPath, fileName);
        }

        /// <summary>
        /// Returns an unique filename in the /wwwroot/Uploads, if failed tries it recursively
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>A unique filename in the /wwwroot/Upload</returns>
        public string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            var newName = Path.GetFileNameWithoutExtension(fileName)
                   + "_"
                   + Guid.NewGuid().ToString().Substring(0, 4)
                   + Path.GetExtension(fileName);

            if (File.Exists(GetFilePath(newName)))
            {
                return GetUniqueFileName(fileName);
            }

            return newName;
        }

        /// <summary>
        /// Copies a form file to /wwwroot/Uploads folder with an unique filename
        /// </summary>
        /// <param name="file"></param>
        /// <returns>Unique filename after copy</returns>
        public string CopyFile(IFormFile file)
        {
            string uniqueFileName = GetUniqueFileName(file.FileName);
            string fileName = GetFilePath(uniqueFileName);
            var fileStream = new FileStream(fileName, FileMode.Create);
            file.CopyTo(fileStream);
            fileStream.Flush();
            fileStream.Close();
            fileStream.Dispose();
            return uniqueFileName;
        }

        public string[] ImageFormats { get; set; } = { ".jpg", ".png", ".jpeg" };

        /// <summary>
        /// Checks if provided form file is an image
        /// </summary>
        /// <param name="vmFile"></param>
        /// <returns>true if provided form file is an image</returns>
        public bool IsImage(IFormFile vmFile)
        {
            bool isImage = false;
            string fileExt = Path.GetExtension(vmFile.FileName);

            foreach (var format in ImageFormats)
            {
                if (format == fileExt)
                {
                    isImage = true;
                }
            }

            return isImage;
        }
    }
}
