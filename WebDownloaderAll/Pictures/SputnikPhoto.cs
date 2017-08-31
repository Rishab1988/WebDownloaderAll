using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using WebDownloaderAll.Common;
using System.Globalization;
using System.IO.Compression;

namespace WebDownloaderAll.Pictures
{

    public sealed class SputnikPhoto : WebPhotoDownload
    {
        protected override string PhotoPath
        {
            get { return @"D:\Rishab\Do Copy\Russia\"; }
        }

        protected override string AutoUrl
        {
            get
            {
                var web = new HtmlWeb();
                var newUrl = web.Load("https://sputniknews.com/photo/").DocumentNode.QuerySelector("a.b-stories__img").GetAttributeValue("href", "");
                return  "https://sputniknews.com" + newUrl;
            }
        }

        protected override List<PhotoDownload> DoImageDownload(PhotoInput photoInput)
        {
            //Console.WriteLine(Resource.readingUrl);
            var web = new HtmlWeb();
            var page = web.Load(photoInput.Url).DocumentNode;

            photoInput.Path = page.QuerySelector("title").InnerText.Trim().RemoveInvalidPathChars().RemoveInvalidSputnikPhotoChars();

            if (photoInput.Path.Contains("This Week in Pictures"))
            {
                photoInput.Path = photoInput.Path + "\\" + DateTime.ParseExact(photoInput.Url.Substring(30, 8), "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MMMM dd yyyy");
            }

            photoInput.FullPath = PhotoPath + photoInput.Path;


            photoInput.Desc = HttpUtility.HtmlDecode(page.QuerySelector("div.b-article__text").InnerText.Trim());
            Photo.WriteRootInfoFile(photoInput);


            var gallery = page.QuerySelector("#gallery").QuerySelectorAll("li");

            var listPhotoDownload = new List<PhotoDownload>();

            foreach (var imageFrame in gallery)
            {
                var photoDownload = new PhotoDownload
                {
                    Url = imageFrame.GetAttributeValue("data-src", ""),
                    Status = false,
                    Title = HttpUtility.HtmlDecode(imageFrame.GetAttributeValue("data-sub-html", ""))
                };
                photoDownload.Name = Photo.GetName(photoDownload.Url);
                photoDownload.Path = Path.GetFullPath(photoInput.FullPath) + "\\" + photoDownload.Name;
                listPhotoDownload.Add(photoDownload);
            }

            return listPhotoDownload;
        }


    }

}
