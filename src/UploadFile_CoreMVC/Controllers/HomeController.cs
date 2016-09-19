using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using UploadFile_CoreMVC.Models;
using System.IO;

namespace UploadFile_CoreMVC.Controllers
{
    public class HomeController : Controller
    {

        private UploadDbContext _context;
        private IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env, UploadDbContext context)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View();
        }
          public IActionResult UploadToDatabase()
        {
            return View();
        }
        public IActionResult LoadFiles()
        {
            return View();
        }
        #region UploadToFolder

        [HttpPost]
        public async Task<IActionResult> UploadToFolder(UploadViewModel model)
        {
            if (model.Files != null && model.Files.Count > 0 && model.Files[0] != null)
            {
                foreach (var file in model.Files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var directory = new DirectoryInfo(_env.WebRootPath + "\\files\\");
                    if (!directory.Exists) { directory.Create(); }
                    var filePath = _env.WebRootPath + "\\files\\" + fileName;
                    using (var fileStream = new FileStream(Path.Combine(filePath), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                }
                ModelState.AddModelError("", "File Uploaded.");
            }
            return RedirectToAction("Index");
        }

        public IActionResult DisplayFilesFromFolder()
        {
            Dictionary<string, string> pathAndExt = new Dictionary<string, string>();

            var filePaths = Directory.GetFiles(_env.WebRootPath + "\\files");
            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileName(filePath);
                var fileExt = Path.GetExtension(filePath);
                var fileType = FileTypes[fileExt];
                pathAndExt.Add(fileName, fileType);
            }
            return View(pathAndExt);
        }

        Dictionary<string, string> FileTypes = new Dictionary<string, string>()
        {
            {".mp4","video" }, {".ogg","video" }, {".webm","video" }, {".mkv", "video" }, {".mpeg","video" }, {".mpg","video" }, {".ogv","video" }, {".ogx","video" }, {".3gp","video" }, {".3g2","video" }, {".m4v","video" }, {".jpe","image"}, {".jpg","image"}, {".gif","image"}, {".png","image"}, {".bmp","image"}, {".mp3","audio"}, {".oga","audio"}, {".m4a","audio"}, {".m4b","audio"}, {".m4r","audio"}, {".m3u","audio"}, {".pls","audio"}, {".opus","audio"}, {".amr","audio"}, {".wav","audio"}, {".lcka","audio"},
        };

        #endregion UploadToFolder

        #region UploadToDb

        [HttpPost]
        public IActionResult UploadToDb(UploadDataViewModel model)
        {
            bool userExists = _context.Users.Where(x => x.UserName == model.UserName).Any();
            if (!userExists)
            {
                User us = new User { Id = Guid.NewGuid().ToString(), UserName = model.UserName };
                _context.Users.Add(us);
                _context.SaveChanges();
            }
            if (model.Files != null && model.Files.Count > 0 && model.Files[0] != null)
            {
                foreach (var file in model.Files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    bool fileExists = _context.UserFiles.Where(x => x.FileName == fileName).Any();
                    if (!fileExists)
                    {
                        var fileType = file.ContentType;
                        Stream stream = file.OpenReadStream();
                        BinaryReader reader = new BinaryReader(stream);
                        var fileContent = reader.ReadBytes((int)file.Length);
                        var userId = _context.Users.Where(x => x.UserName == model.UserName).Select(x => x.Id).FirstOrDefault();
                        UserFiles uf = new UserFiles
                        {
                            Id = Guid.NewGuid().ToString(),
                            FileName = fileName,
                            FileType = fileType,
                            UserFile = fileContent,
                            UserId = userId
                        };
                        _context.UserFiles.Add(uf);
                        _context.SaveChanges();
                        ModelState.AddModelError("", "File Uploaded.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "File exists.");
                    }
                }
            }
            return RedirectToAction("UploadToDatabase");
        }

        [HttpPost]
        public IActionResult DisplayFilesFromDb(UploadDataViewModel model)
        {
            bool userExists = _context.Users.Where(x => x.UserName == model.UserName).Any();
            List<LoadFileViewModel> ufls = new List<LoadFileViewModel>();
            if (userExists)
            {
                var userId = _context.Users.Where(x => x.UserName == model.UserName).Select(x => x.Id).FirstOrDefault();
                var userFiles = _context.UserFiles.Where(x => x.UserId == userId).ToList();
                foreach (var userFile in userFiles)
                {
                    string type = null;
                    int index = userFile.FileType.IndexOf('/');
                    if (index > 0) { type = userFile.FileType.Substring(0, index); }
                    ufls.Add(new LoadFileViewModel() { FileName = userFile.FileName, FileType = type, Id = userFile.Id });
                }
                return View(ufls);
            }
            else
            {
                ModelState.AddModelError("", "User Not Exists.");
                return RedirectToAction("LoadFiles", "Home");
            }
        }

        public IActionResult Media(string id)
        {
            var userFile = _context.UserFiles.Where(x => x.Id == id).FirstOrDefault();
            long fSize = userFile.UserFile.Length;
            long startbyte = 0;
            long endbyte = fSize - 1;
            int statusCode = 200;
            var hRange = Request.Headers["range"].ToString();
            if (!string.IsNullOrEmpty(hRange))
            {
                //Get the actual byte range from the range header string, and set the starting byte.
                string[] range = hRange.Split(new char[] { '=', '-' });
                startbyte = Convert.ToInt64(range[1]);
                if (range.Length > 2 && range[2] != "") endbyte = Convert.ToInt64(range[2]);
                //If the start byte is not equal to zero, that means the user is requesting partial content.
                if (startbyte != 0 || endbyte != fSize - 1 || range.Length > 2 && range[2] == "")
                { statusCode = 206; }//Set the status code of the response to 206 (Partial Content) and add a content range header.                                    
            }
            long desSize = endbyte - startbyte + 1;
            //Headers
            Response.StatusCode = statusCode;
            Response.Headers.Add("Content-Accept", userFile.FileType);
            Response.Headers.Add("Content-Length", desSize.ToString());
            Response.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", startbyte, endbyte, fSize));

            var fs = new MemoryStream(userFile.UserFile, (int)startbyte, (int)desSize);
            return new FileStreamResult(fs, userFile.FileType);
        }

        #endregion UploadToDb
    }
}
