using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Pictures
{
    public class PhotoInput
    {
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Url { get; set; }
        public string Desc { get; set; }
    }

    public class PhotoDownload
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool Status { get; set; }
    }

    internal class PhotoInputException : Exception
    {
        public PhotoInputException(string message)
            : base(message)
        {
        }

        public PhotoInputException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }


    public static class Photo
    {
        internal static string GetName(string url)
        {
            var uri = new Uri(url);
            var name = Path.GetFileName(uri.LocalPath).Replace("\r\nRead more:", "");
            return name;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        internal static bool WriteRootInfoFile(PhotoInput photoInput)
        {
            var directoryInfo = new DirectoryInfo(photoInput.FullPath);

            try
            {
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
            }
            catch (IOException exception)
            {
                throw new PhotoInputException("An error ocured while creating directory", exception);
            }
            var writer = File.CreateText(photoInput.FullPath + "\\root.info");
            var isWriteComplete = false;
            try
            {
                writer.WriteLine("Path: ");
                writer.WriteLine(photoInput.Path);
                writer.WriteLine();
                writer.WriteLine("Url: ");
                writer.WriteLine(photoInput.Url);
                writer.WriteLine();
                if (!String.IsNullOrEmpty(photoInput.Desc))
                {
                    writer.WriteLine("Desc: ");
                    writer.WriteLine(photoInput.Desc);
                    var picasaFile = photoInput.FullPath + "\\.picasa.ini";
                    File.WriteAllText(picasaFile, "[Picasa]\r\ndescription=" + photoInput.Desc);
                    File.SetAttributes(picasaFile, FileAttributes.System);
                    File.SetAttributes(picasaFile, FileAttributes.Hidden);
                }

                isWriteComplete = true;
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                writer.Close();
            }

            return isWriteComplete;
        }


        internal static List<PhotoInput> TakeUserInput()
        {
            var photoInputCollection = new List<PhotoInput>();
            Console.WriteLine(Resource.inputUrl);

            string url;

            var urlCollection = new List<string>();

            while (((url = Console.ReadLine()) != null) && (url != ""))
            {
                urlCollection.Add(url);

                photoInputCollection.Add(new PhotoInput { Url = url });
            }

            if (urlCollection.Count == 0)
                throw new PhotoInputException("Invalid url provided");

            Console.Clear();
            return photoInputCollection;
        }

        internal static PhotoInput TakeUserInput(string photoPath, bool showPathOption = true)
        {
            var photoInput = new PhotoInput();

            if (showPathOption)
            {
                Console.WriteLine(Resource.inputFolderPath);
                var dirPath = Console.ReadLine();

                if (string.IsNullOrEmpty(dirPath))
                    throw new PhotoInputException("Invalid input values");

                dirPath = dirPath.RemoveInvalidPathChars();

                dirPath = photoPath + dirPath;

                var directoryInfo = new DirectoryInfo(dirPath);

                try
                {
                    if (!directoryInfo.Exists)
                        directoryInfo.Create();
                }
                catch (IOException exception)
                {
                    throw new PhotoInputException("An error ocured while creating directory", exception);
                }


                photoInput.Path = dirPath;
            }

            Console.WriteLine(Resource.inputUrl);
            var url = Console.ReadLine();

            if (string.IsNullOrEmpty(url))
                throw new PhotoInputException("Invalid url provided");

            photoInput.Url = url;

            Console.Clear();
            return photoInput;
        }


        internal static void DoDownload(List<PhotoDownload> listPhotoDownloads)
        {
            var count = listPhotoDownloads.Count();
            Console.Write(Resource.withItemsAt, count);
            var client = new WebClient();
            StreamWriter streamWriter;
            for(int i = 0; i <count ; i++ )// (var photoDownload in listPhotoDownloads)
            {
                var photoDownload = listPhotoDownloads[i];
                client.DownloadFile(photoDownload.Url, photoDownload.Path);
                if (Path.GetExtension(photoDownload.Name) == ".png")
                {
                    streamWriter = File.CreateText(photoDownload.Path + ".info");
                    streamWriter.Write(photoDownload.Title);
                    streamWriter.Close();
                }
                else
                {
                    var shellFile = ShellObject.FromParsingName(photoDownload.Path);
                    var propWriter = shellFile.Properties.GetPropertyWriter();
                    propWriter.WriteProperty(SystemProperties.System.Title, photoDownload.Title);
                    propWriter.Close();
                }
                photoDownload.Status = true;
                //Console.WriteLine(Resource.downloadComplete2, photoDownload.Url, photoDownload.Name);
                var percentage = ((i + 1) * 100) / count;
                Console.Write("{0} %", percentage);

                Console.Write(new string('\b', percentage.ToString().Length + 2));
            }

            var jsonSerialiser = new JavaScriptSerializer();
            streamWriter = File.CreateText(Path.GetDirectoryName(listPhotoDownloads.First().Path) + "\\data.json");
            streamWriter.Write(jsonSerialiser.Serialize(listPhotoDownloads));
            streamWriter.Close();

            //Console.WriteLine(Resource.downloadComplete);
        }
    }
}