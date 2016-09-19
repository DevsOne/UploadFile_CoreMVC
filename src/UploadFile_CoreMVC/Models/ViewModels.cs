using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UploadFile_CoreMVC.Models
{
    public class User
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        public ICollection<UserFiles> UserFiles { get; set; }
    }
    public class UserFiles
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public byte[] UserFile { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public User User { get; set; }

    }
    public class UploadViewModel
    {
        [Display(Name = "File")]
        public List<IFormFile> Files { get; set; }
    }

    public class UploadDataViewModel
    {

        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Display(Name = "File")]
        public List<IFormFile> Files { get; set; }
    }
    public class LoadFileViewModel
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Id { get; set; }
    }
}
