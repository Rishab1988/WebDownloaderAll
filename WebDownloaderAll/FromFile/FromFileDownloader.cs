using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.FromFile
{

    public class FileInputData
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class FileInput
    {
        public string File { get; set; }
        public List<FileInputData> FileInputData { get; set; }
    }

    public enum FileType
    {
        Music,
        Other
    }

    public class Extensions
    {
        public List<string> GetFileTypeExtensions()
        {
            throw new NotImplementedException();
        }
    }

    public class FromFileDownloader : IWebDownload
    {
        public void DoStackCall()
        {
            var urlCollection = GetUrls(GetUserInput());
            DoCreateListFile(urlCollection);
            DoDownload(urlCollection);
        }

        private static string BaseDirPath
        {
            get
            {
                return @"D:\Rishab\Do Copy\";
            }
        }

        public static string DirPath(FileType fileType)
        {

            switch (fileType)
            {
                case FileType.Other:
                    return BaseDirPath;
                case FileType.Music:
                    return BaseDirPath + "Music\\";
                default:
                    return BaseDirPath;
            }
        }


        private List<FileInput> GetUserInput()
        {
            string file;

            var fileInputCollection = new List<FileInput>();
            Console.WriteLine(Resource.inputUrl);
            while (((file = Console.ReadLine()) != null) && (file != ""))
            {
                fileInputCollection.Add(new FileInput { File = @"D:\Rishab\Task2\downloadming\" + file + ".txt" });
            }

            return fileInputCollection;
        }


        private List<FileInput> GetUrls(List<FileInput> fileInputCollection)
        {
            foreach (var fileInput in fileInputCollection)
            {
                fileInput.FileInputData = File.ReadAllLines(fileInput.File).Where(x => x.Length > 0).Select(y => new FileInputData { Url = y }).ToList();
            }
            return fileInputCollection;
        }

        void DoCreateListFile(List<FileInput> fileInputCollection)
        {
            string fileName = string.Format(@"{0}_{1}.txt", Guid.NewGuid(), DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            File.WriteAllLines(@"D:\Rishab\Task2\Down Data\" + fileName, fileInputCollection.Select(x => x.File));
        }

        private void DoDownload(List<FileInput> fileInputCollection)
        {
            var client = new WebClient();
            Console.Clear();

            if (fileInputCollection.Exists(x => x.FileInputData.Count > 0))
            {
                if (!Directory.Exists(DirPath(FileType.Music) + "Album"))
                    Directory.CreateDirectory(DirPath(FileType.Music) + "Album");
            }

            var count = fileInputCollection.Count();
            for (int i = 0; i <count; i++)  //foreach (var fileInput in fileInputCollection)
            {
                var fileInput = fileInputCollection.ElementAt(i);

                //Console.WriteLine();
                //Console.WriteLine(Resource.openingFileWithItems, fileInput.File, fileInput.FileInputData.Count());
               
                var itemCount = fileInput.FileInputData.Count();

                Console.Clear();
                Console.Write(Resource.readingUrlFromwithItemsAt, i + 1, count, itemCount);
                Console.Write("{0} %", 0);
                Console.Write(new string('\b', 1 + 2));
                for (int k = 0;  k < itemCount; k++) //foreach (var fileInputData in fileInput.FileInputData)
                {

                    var fileInputData = fileInput.FileInputData.ElementAt(k);

                    fileInputData.Name = fileInputData.Url.GetNameFromUrl();
                    if (fileInput.File.IndexOf("Album") > 0)
                    {
                        if (!File.Exists(DirPath(FileType.Music) + "Album\\" + fileInputData.Name))
                            client.DownloadFile(fileInputData.Url, DirPath(FileType.Music) + "Album\\" + fileInputData.Name);
                    }
                    else
                    {
                        if (File.Exists(DirPath(FileType.Music) + fileInputData.Name))
                        {
                            //Console.WriteLine("\t" + Resource.downloadComplete1, fileInputData.Name);
                            continue;
                        }
                        
                        try
                        {
                            client.DownloadFile(fileInputData.Url, DirPath(FileType.Music) + fileInputData.Name);
                            //Console.WriteLine("\t" + Resource.downloadComplete1, fileInputData.Name);
                            
                        }
                        catch
                        {
                            //Console.WriteLine("\t" + Resource.error, fileInputData.Name);
                            File.AppendAllText(@"D:\WDAError.txt", fileInputData.Url + "\r\n");
                        }
                    }
                    var percentage = ((k + 1) * 100) / itemCount;
                    Console.Write("{0} %", percentage);
                    Console.Write(new string('\b', percentage.ToString().Length + 2));
                }
            }

            Console.WriteLine(Resource.downloadComplete);
        }

    }
}
