using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Pictures
{
    public class RbthPhoto : WebPhotoDownload
    {
        protected override List<PhotoDownload> DoImageDownload(PhotoInput photoInput) {
            Console.WriteLine(Resource.readingUrl);
            var web = new HtmlWeb();
            var page = web.Load(photoInput.Url).DocumentNode;

            photoInput.Path = page.QuerySelector("h1.container_12.title").InnerText.Trim().RemoveInvalidPathChars();
            photoInput.FullPath = PhotoPath + photoInput.Path;
            photoInput.Desc = page.QuerySelector("div.container_12.lead").InnerText.Trim();
            Photo.WriteRootInfoFile(photoInput);
            var listPhotoDownload = new List<PhotoDownload>();
            //var gallery = page.QuerySelectorAll("div.tumblr.hide-mobile");

            var gallery = page.QuerySelectorAll("section.container_12.content div.single_photo");

            foreach (var imageFrame in gallery) {
                var photoDownload = new PhotoDownload
                {
                    Url = imageFrame.QuerySelector("img.article_img").GetAttributeValue("src", ""), //imageFrame.GetAttributeValue("data-content", ""),
                    Status = false,
                    Title = HttpUtility.HtmlDecode(imageFrame.QuerySelector("div.text").InnerText) //HttpUtility.HtmlDecode(imageFrame.GetAttributeValue("data-caption", ""))
                };
                photoDownload.Name = Photo.GetName(photoDownload.Url);
                photoDownload.Path = Path.GetFullPath(photoInput.FullPath) + "\\" + photoDownload.Name;
                listPhotoDownload.Add(photoDownload);
            }


            return listPhotoDownload;
        }
    }
}
