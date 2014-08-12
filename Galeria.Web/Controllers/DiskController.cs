using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BootstrapMvcSample.Controllers;
using Galeria.Domain;
using Galeria.Domain.Services;
using Galeria.Web.Models;
using Galeria.Web.Utils;
using NHibernate.Mapping;
using File = Galeria.Domain.File;

namespace Galeria.Web.Controllers
{
    public class DiskController : BootstrapBaseController
    {
        private readonly IReadOnlyRepository _readOnlyRepository;
        private readonly IWriteOnlyRepository _writeOnlyRepository;
        

        public DiskController(IWriteOnlyRepository writeOnlyRepository, IReadOnlyRepository readOnlyRepository)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
        }


        [HttpGet]
        public ActionResult ListAllContent()
        {
             var userContent = new List<DiskContentModel>();

            //var actualPath = Session["ActualPath"].ToString();
            var actualFolder = Session["ActualFolder"].ToString();
            var userFiles = _readOnlyRepository.First<Account>(x => x.EMail == User.Identity.Name).Files;

           
            foreach (var file in userFiles)
            {
                if (file == null)
                    continue;

                var fileFolder = file.Url.Split('\\');

                if (!file.IsArchived && fileFolder[fileFolder.Length-2].Equals(actualFolder))
                    userContent.Add(Mapper.Map<DiskContentModel>(file));
            }

            if (userContent.Count == 0)
            {
                userContent.Add(new DiskContentModel
                {
                    Id = 0,
                    ModifiedDate = DateTime.Now.Date,
                    Name = "You can now add files to your account",
                    Type = "none"
                });
            }

            return View(userContent);
        }

        [HttpGet]
        public PartialViewResult FileUploadModal()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase fileControl, string clientDateTime)
        {
            if (fileControl == null)
            {
                Error("There was a problem uploading the file :( , please try again!!!");
                return RedirectToAction("ListAllContent");
            }

            var fileSize = fileControl.ContentLength;

            if (fileSize > 10485760)
            {
                Error("The file must be of 10 MB or less!!!");
                return RedirectToAction("ListAllContent");
            }

            var userData = _readOnlyRepository.First<Account>(x => x.EMail == User.Identity.Name);
            var actualPath = Session["ActualPath"].ToString();
            var fileName = Path.GetFileName(fileControl.FileName);

            var serverFolderPath = Server.MapPath("~/Imagenes/UploadedFiles/" + actualPath + "/");        
            var path = Path.Combine(serverFolderPath, fileName);

            var fileInfo = new DirectoryInfo(serverFolderPath + fileName);

            if (fileInfo.Exists)
            {
                var bddInfo = userData.Files.FirstOrDefault(f => f.Name == fileName);
                bddInfo.ModifiedDate = DateTime.Now;
                bddInfo.Type = fileControl.ContentType;
                bddInfo.FileSize = fileSize;
            }
            else
            {
                userData.Files.Add(new File
                {
                    Name = fileName,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    FileSize = fileSize,
                    Type = fileControl.ContentType,
                    Url = serverFolderPath,
                    IsArchived = false,
                    IsDirectory = false
                });
            }

            fileControl.SaveAs(path);
            _writeOnlyRepository.Update(userData);

            Success("File uploaded successfully!!! :D");
            return RedirectToAction("ListAllContent");
        }      

        public ActionResult DeleteFile(int fileId)
        {
            var userData = _readOnlyRepository.First<Account>(a => a.EMail == User.Identity.Name);
            var fileToDelete = userData.Files.FirstOrDefault(f => f.Id == fileId);

            if (fileToDelete != null)
            {

                System.IO.File.Delete(fileToDelete.Url + fileToDelete.Name);

                fileToDelete.IsArchived = true;

                _writeOnlyRepository.Update(userData);
            }

            return RedirectToAction("ListAllContent");
        }

        [HttpPost]
        public ActionResult CreateFolder(string folderName, string clientDateTime)
        {
            if (folderName.Length > 25)
            {
                Error("Folder name should be 25 characters maximum!!!");
                return RedirectToAction("ListAllContent");
            }

            var userData = _readOnlyRepository.First<Account>(x => x.EMail == User.Identity.Name);

            if (folderName == userData.EMail)
            {
                Error("Folder already exists!!!");
                return RedirectToAction("ListAllContent");
            }

            var actualPath = Session["ActualPath"].ToString();
            var serverFolderPath = Server.MapPath("~/Imagenes/UploadedFiles/" + actualPath + "/" + folderName);

            var folderInfo = new DirectoryInfo(serverFolderPath);

            if (folderInfo.Exists)
            {
                Error("Folder already exists!!!");
                return RedirectToAction("ListAllContent");
            }


            userData.Files.Add(new File
            {
                Name = folderName,
                CreatedDate = DateTime.Now,
                FileSize = 0,
                IsArchived = false,
                IsDirectory = true,
                ModifiedDate = DateTime.Now,
                Type = "",
                Url = Server.MapPath("~/Imagenes/UploadedFiles/" + actualPath)
            });

            var result = Directory.CreateDirectory(serverFolderPath);

            if (!result.Exists)
                Error("The folder was not created!!! Try again please!!!");
            else
            {
                Success("The folder was created successfully!!!");
                _writeOnlyRepository.Update(userData);
            }

            return RedirectToAction("ListAllContent");
        }

        public void DeleteFolder(long folderId)
        {
            var userData = _readOnlyRepository.First<Account>(a => a.EMail == User.Identity.Name);
            var folderToDelete = userData.Files.FirstOrDefault(f => f.Id == folderId);


            var userFiles = userData.Files.Where(t => t.Url.Contains(folderToDelete.Name));

            foreach (var file in userFiles)
            {
                if (file == null)
                    continue;

                if (file.IsDirectory)
                    DeleteFolder(file.Id);

                var fileFolderArray = file.Url.Split('/');
                var fileFolder = fileFolderArray.Length > 1 ? fileFolderArray[fileFolderArray.Length - 2] : fileFolderArray.FirstOrDefault();

                if (!file.IsArchived && fileFolder.Equals(folderToDelete.Name) &&
                    !string.Equals(file.Name, folderToDelete.Name))
                {

                    file.IsArchived = true;
                    _writeOnlyRepository.Update(userData);
                }
            }

            folderToDelete.IsArchived = true;
            _writeOnlyRepository.Update(userData);
        }
        
        public ActionResult ListFolderContent(string folderName)
        {
            Session["ActualPath"] += folderName + "/";
            Session["ActualFolder"] = folderName; 
            return RedirectToAction("ListAllContent");
        }
        public ActionResult ListFolderContent2(string path, string folderName)
        {
            if (path == null)
            {
                path = "";
            }
            Session["ActualPath"] = path;
            Session["ActualFolder"] = folderName;
            return RedirectToAction("ListAllContent");
        }

        public ActionResult ListAllContentRoot()
        {
           
            return RedirectToAction("ListAllContent");
        }

        public ActionResult DownloadFile(int fileId)
        {
            var fileData =
               _readOnlyRepository.First<Account>(x => x.EMail == User.Identity.Name)
                   .Files.FirstOrDefault(f => f.Id == fileId);

            var template_file = System.IO.File.ReadAllBytes(fileData.Url + "/" + fileData.Name);

            return new FileContentResult(template_file, fileData.Type)
            {
                FileDownloadName = fileData.Name
            };
        }
        [HttpGet]       
        public void RenameFolder(long objectId,string oldObjectName, string newObjectName, string clientDateTime2)
        {
            var userData = _readOnlyRepository.First<Account>(a => a.EMail == User.Identity.Name);
            var fileData = userData.Files.FirstOrDefault(f => f.Id == objectId);

            var userFiles = userData.Files.Where(t => t.Url.Contains(fileData.Name));
            
            var clientDate = Convert.ToDateTime(clientDateTime2);
            var newFoldUrl = string.IsNullOrEmpty(fileData.Url) || string.IsNullOrWhiteSpace(fileData.Url)
                ? newObjectName + "/"
                : fileData.Url.Replace(oldObjectName, newObjectName) + fileData.Name + "/";

           
            foreach (var file in userFiles)
            {
                if (file == null)
                    continue;

                if (file.IsDirectory)
                {
                    RenameFolder(file.Id,oldObjectName,newObjectName,clientDateTime2);
                }
                else
                {
                    //Copy the object
                    var newUrl = file.Url.Replace(oldObjectName, newObjectName) + file.Name;

                   
                    //Delete the original                  

                    file.ModifiedDate = clientDate;
                    file.Url = file.Url.Replace(oldObjectName, newObjectName);
                    _writeOnlyRepository.Update(file);
                }
            }//fin foreach
           
            var newFolderUrl = fileData.Url.Replace(oldObjectName, newObjectName);
            fileData.Url = newFolderUrl;
            
            _writeOnlyRepository.Update(fileData);
        }
      
        public ActionResult ShowFile(int id)
        {
            var ar = new MostrarArchivosModel();
            ar.id = id;
            ar.file = Download(id);
            var userData = _readOnlyRepository.First<Account>(a => a.EMail == User.Identity.Name);
            var fileData = userData.Files.FirstOrDefault(f => f.Id == id);
            ar.filename = fileData.Name;
            return PartialView(ar);
        }

        public FileContentResult Download(long fileId)
        {
            var fileData =
                _readOnlyRepository.First<Account>(x => x.EMail == User.Identity.Name)
                    .Files.FirstOrDefault(f => f.Id == fileId);

            var template_file = System.IO.File.ReadAllBytes(fileData.Url + "/" + fileData.Name);

            return new FileContentResult(template_file, fileData.Type)
            {
                FileDownloadName = fileData.Name
            };
        }
        public ActionResult Checked(long id)
        {
            return View();
        }
    }
}
