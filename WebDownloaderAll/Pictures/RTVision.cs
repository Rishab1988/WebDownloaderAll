using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Pictures
{
    public class RTVision : WebPhotoDownload
    {
        protected override string PhotoPath
        {
            get { return @"D:\Rishab\Do Copy\RT InVision\"; }
        }
        
        protected override List<PhotoDownload> DoImageDownload(PhotoInput photoInput)
        {
            //Console.WriteLine(Resource.readingUrl);
            var web = new HtmlWeb();
            var page = web.Load(photoInput.Url).DocumentNode;

            photoInput.Path = page.QuerySelector("h1.category-heading").InnerText.Trim().RemoveInvalidPathChars();
            photoInput.FullPath = PhotoPath + photoInput.Path;
            Photo.WriteRootInfoFile(photoInput);

            var gallery = page.QuerySelectorAll("figure.media a");

            var listPhotoDownload = new List<PhotoDownload>();

            foreach (var imageFrame in gallery)
            {
                var photoDownload = new PhotoDownload
                {
                    Url = imageFrame.GetAttributeValue("data-article", ""),
                    Status = false,
                    Title = HttpUtility.HtmlDecode(imageFrame.GetAttributeValue("data-title", ""))
                };
                photoDownload.Name = Photo.GetName(photoDownload.Url);
                photoDownload.Path = Path.GetFullPath(photoInput.FullPath) + "\\" + photoDownload.Name;
                listPhotoDownload.Add(photoDownload);
            }

            return listPhotoDownload;
        }
    }
}
