#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.IO;
using System.Web;
using DotNetNuke.Entities.Host;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core
{
    public class FileUpload : BaseHandler
    {
        /// <summary>
        ///     Maximum upload size in bytes
        ///     default: 0 = unlimited
        /// </summary>
        protected int MaxUploadSize = 0;

        protected override object HandleAction(HttpContext context, HotcakesApplication hccApp)
        {
            if (context.User.Identity.IsAuthenticated == false)
            {
                // not found
                context.Response.StatusCode = 404;
                context.Response.End();
                return null;
            }

            if (context.Request.Files.Count <= 0)
            {
                return null;
            }

            try
            {
                var file = context.Request.Files[0];

                var fileName = context.Request.Params["filename"];
                var optUniqueId = context.Request.Params["optUniqueId"];

                var prePath = GetInitialPathOfUploadFolder(hccApp);
                var orderProductPath = prePath + "\\" + optUniqueId;

                var subPath = string.Format("OrderFiles/{0}/{1}", orderProductPath,
                    Path.GetFileName(Text.CleanFileName(fileName)));
                var fullFilePath = DiskStorage.GetStoreDataPhysicalPath(hccApp.CurrentStore.Id, subPath);

                //check if path directory exists
                var dirPath = Path.GetDirectoryName(fullFilePath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var downloadPath =
                    VirtualPathUtility.ToAbsolute(DiskStorage.GetStoreDataVirtualPath(hccApp.CurrentStore.Id, subPath));

                var extension = Path.GetExtension(fullFilePath);
                if (Host.AllowedExtensionWhitelist.IsAllowedExtension(extension) == false)
                {
                    return new UploadResponse
                    {
                        StatusCode = 413,
                        Message = "Invalid File Extension"
                    };
                }

                //Check if file is sent in chunks and chunk is present in request
                var totalChunks = context.Request["chunks"] ?? string.Empty;
                var chunks = -1;
                if (!int.TryParse(totalChunks, out chunks))
                {
                    chunks = -1;
                }

                var chunkNo = context.Request["chunk"] ?? string.Empty;
                var chunk = -1;
                if (!int.TryParse(chunkNo, out chunk))
                {
                    chunk = -1;
                }

                //If there are no chunks sent then the file is sent as one
                //Indicates Plain HTML4 Upload & 1 single file
                if (chunks == -1)
                {
                    if (MaxUploadSize == 0 || context.Request.ContentLength <= MaxUploadSize)
                    {
                        if (OnUploadChunk(file.InputStream, 0, 1, fullFilePath) == false)
                        {
                            return new UploadResponse
                            {
                                StatusCode = 500,
                                Message = "Unable to write file"
                            };
                        }
                    }
                    else
                    {
                        return new UploadResponse
                        {
                            StatusCode = 413,
                            Message = "Uploaded File is too large"
                        };
                    }

                    //Return the final virtual path of uploaded file
                    return new UploadResponse
                    {
                        StatusCode = 200,
                        Message = downloadPath
                    };
                }
                //File is sent in chunk, so full size of file & chunk is unknown
                if (chunk == 0 && MaxUploadSize > 0 && context.Request.ContentLength*(chunks - 1) > MaxUploadSize)
                {
                    return new UploadResponse
                    {
                        StatusCode = 413,
                        Message = "Uploaded File is too large"
                    };
                }

                //First Chunk
                if (chunk == 0)
                {
                    // TODO: Delete old file on re-uploading of same file again with 0th chunk
                    if (File.Exists(fullFilePath))
                    {
                        File.SetAttributes(fullFilePath, FileAttributes.Normal);
                        File.Delete(fullFilePath);
                    }
                }

                //n'th  chunk
                if (OnUploadChunk(file.InputStream, chunk, chunks, fullFilePath) == false)
                {
                    return new UploadResponse
                    {
                        StatusCode = 500,
                        Message = "Unable to write file"
                    };
                }

                //last chunk
                if (chunk == chunks - 1)
                {
                    //return the file's virtual download path
                    return new UploadResponse
                    {
                        StatusCode = 200,
                        Message = downloadPath
                    };
                }

                //If no response is sent yet send the success response
                return new UploadResponse
                {
                    StatusCode = 200,
                    Message = downloadPath
                };
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return new UploadResponse
                {
                    StatusCode = 500,
                    Message = "Unable to write file"
                };
            }
        }

        private string GetInitialPathOfUploadFolder(HotcakesApplication hccApp)
        {
            var currentCart = hccApp.OrderServices.EnsureShoppingCart();
            return currentCart != null ? currentCart.bvin : string.Empty;
        }

        /// <summary>
        ///     Fired as the upload happens
        /// </summary>
        /// <param name="chunkStream"></param>
        /// <param name="chunk"></param>
        /// <param name="chunks"></param>
        /// <param name="uploadedFilename"></param>
        /// <returns>return true on success false on failure</returns>
        /// <summary>
        ///     Stream each chunk to a file and effectively append it.
        /// </summary>
        protected bool OnUploadChunk(Stream chunkStream, int chunk, int chunks, string uploadedFilename)
        {
            if (chunk == 0)
            {
                if (File.Exists(uploadedFilename))
                {
                    File.Delete(uploadedFilename);
                }
            }

            Stream stream = null;
            try
            {
                stream = new FileStream(uploadedFilename, chunk == 0 ? FileMode.CreateNew : FileMode.Append);
                chunkStream.CopyTo(stream, 16384);
            }
            catch
            {
                //UnableToWriteOutFile
                return false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return true;
        }
    }


    public class UploadResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}