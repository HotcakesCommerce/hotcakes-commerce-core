#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Areas.AdminContent.Models;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Areas.AdminContent.Controllers
{
    [Serializable]
    public class FileManagerController : BaseAdminController
    {
        private BreadCrumbViewModel BuildBreadCrumbs(string path)
        {
            var result = new BreadCrumbViewModel();
            result.HideHomeLink = true;

            var items = new List<BreadCrumbItem>();

            var workingPath = path.TrimEnd('\\');

            var finished = false;
            while (finished == false)
            {
                workingPath = workingPath.TrimEnd('\\');
                if (workingPath.Length < 1)
                {
                    finished = true;
                    break;
                }
                var dir = Path.GetFileName(workingPath);

                items.Add(new BreadCrumbItem
                {
                    Link = Url.Content("~/DesktopModules/Hotcakes/API/mvc/filemanager?path=" + workingPath),
                    Name = dir,
                    Title = dir
                });
                workingPath = Path.GetDirectoryName(workingPath);
            }
            items.Add(new BreadCrumbItem
            {
                Link = Url.Content("~/DesktopModules/Hotcakes/API/mvc/filemanager"),
                Name = "Root",
                Title = "Root"
            });

            for (var i = items.Count - 1; i >= 0; i--)
            {
                result.Items.Enqueue(items[i]);
            }
            return result;
        }

        //
        // GET: /AdminContent/FileManager/        
        public ActionResult Index()
        {
            var path = Request.QueryString["path"] ?? string.Empty;
            var cleanPath = DiskStorage.FileManagerCleanPath(path);

            var model = new FileManagerViewModel(cleanPath)
            {
                Directories = DiskStorage.FileManagerListDirectories(HccApp.CurrentStore.Id, cleanPath),
                Files = DiskStorage.FileManagerListFiles(HccApp.CurrentStore.Id, cleanPath),
                BreadCrumbs = BuildBreadCrumbs(cleanPath),
                BasePreviewUrl = DiskStorage.GetStoreDataPhysicalPath(HccApp.CurrentStore.Id)
            };

            return View(model);
        }

        [HccHttpPost]
        public ActionResult CreateDirectory()
        {
            var path = Request.Form["path"] ?? string.Empty;
            path = DiskStorage.FileManagerCleanPath(path);
            var newDirName = Request.Form["newdirectoryname"] ?? string.Empty;
            newDirName = DiskStorage.FileManagerCleanPath(newDirName);

            var fullPath = path + "\\" + newDirName;
            DiskStorage.FileManagerCreateDirectory(HccApp.CurrentStore.Id, fullPath);

            var destination = Url.Content("~/DesktopModules/Hotcakes/API/mvc/filemanager?path=" + path);
            return new RedirectResult(destination);
        }

        [HccHttpPost]
        public ActionResult DeleteDirectory()
        {
            var path = Request.Form["path"] ?? string.Empty;
            path = DiskStorage.FileManagerCleanPath(path);
            var deletePath = Request.Form["deletepath"] ?? string.Empty;
            deletePath = DiskStorage.FileManagerCleanPath(deletePath);

            if (!DiskStorage.FileManagerIsSystemPath(deletePath))
            {
                DiskStorage.FileManagerDeleteDirectory(HccApp.CurrentStore.Id, deletePath);
            }

            var destination = Url.Content("~/DesktopModules/Hotcakes/API/mvc/filemanager?path=" + path);
            return new RedirectResult(destination);
        }

        [HccHttpPost]
        public ActionResult Upload()
        {
            var path = Request.Form["path"] ?? string.Empty;
            path = DiskStorage.FileManagerCleanPath(path);

            try
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    if (file.ContentLength > 0)
                    {
                        var completeFileName = file.FileName;
                        var nameSmall = Path.GetFileName(completeFileName);
                        var fullPathAndName = path + "\\" + nameSmall;
                        DiskStorage.FileManagerCreateFile(HccApp.CurrentStore.Id, fullPathAndName, file);
                    }
                }
            }
            catch (Exception ex)
            {
                FlashFailure(ex.Message);
                EventLog.LogEvent(ex);
            }

            var destination = Url.Content("~/DesktopModules/Hotcakes/API/mvc/filemanager?path=" + path);
            return new RedirectResult(destination);
        }

        [HccHttpPost]
        public ActionResult DeleteFile()
        {
            var path = Request.Form["path"] ?? string.Empty;
            path = DiskStorage.FileManagerCleanPath(path);
            var fileName = Request.Form["filename"] ?? string.Empty;
            fileName = DiskStorage.FileManagerCleanPath(fileName);

            var fullPath = path + "\\" + fileName;
            DiskStorage.FileManagerDeleteFile(HccApp.CurrentStore.Id, fullPath);

            var destination = Url.Content("~/DesktopModules/Hotcakes/API/mvc/filemanager?path=" + path);
            return new RedirectResult(destination);
        }
    }
}