using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Music.Writer
{
    public class Downloadming : IWebDownload
    {
        private static string DirPath {
            get { return @"D:\Rishab\Task2\downloadming"; }
        }

        private static string Url {
            get { return "http://downloadming.tv/bollywood-mp3-"; }
        }


        private static IEnumerable<string> NotRecommenedUrlStrings {
            get {
                var notRecommenedUrlStrings = new[] { "(320%20Kbps)", ".zip", "mix" };//, "temp"
                return notRecommenedUrlStrings;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<string> UrlStrings {
            get {
                var urlStrings = new string[27];
                urlStrings[0] = "0-9";

                var k = 1;
                for (var i = 97; i <= 122; i++, k++) {
                    urlStrings[k] = ((char)i).ToString();
                }

                return urlStrings;
            }
        }

        private static IEnumerable<string> LoadedFiles(string urlString) {

            List<string> loadedFiles;
            if (urlString == "0-9") {
                loadedFiles = Directory.GetFiles(DirPath, "*.txt", SearchOption.TopDirectoryOnly)
                    .ToList();
            }
            else {
                loadedFiles = Directory.GetFiles(DirPath, urlString + "*.txt", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension)
                    .ToList();
            }

            return loadedFiles;


            //return
            //    Directory.GetFiles(DirPath, urlString + "*", SearchOption.TopDirectoryOnly)
            //        .Select(Path.GetFileNameWithoutExtension)
            //        .ToArray();
        }


        public void DoStackCall() {
            DoAlbumSearchCall();
        }

        private static void DoAlbumSearchCall() {
            Console.Clear();
            Console.WriteLine(Resource.longProcess);
            foreach (var urlString in UrlStrings) {
                var url = urlString;
                Task.Run(() => DoAlbumSearch(Url, url));
                //DoAlbumSearch(Url, url);
            }
            //DoAlbumSearch(Url,"q");
        }

        private static void DoAlbumSearch(string url, string urlString) {
            url = url + urlString;
            //Console.WriteLine(Resource.readingUrl + ": " + urlString);
            var web = new HtmlWeb();
            try {
                var listMusicInfo = web.Load(url)
                .DocumentNode.QuerySelectorAll("div.azindex ul a")
                .Select(album => new MusicInfo()
                {
                    AlbumName = album.InnerText.RemoveInvalidPathChars().RemoveInvalidDownloadmingAlbumChars(),
                    //AlbumUrl = album.GetAttributeValue("href", ""),
                    SongUrls = web.Load(album.GetAttributeValue("href", ""))
                        .DocumentNode.QuerySelectorAll("div.entry a")
                        .Where(x => x.GetAttributeValue("rel", "") == "")
                        .Select(x => x.GetAttributeValue("href", ""))
                        .Where(x => NotRecommenedUrlStrings.All(y => !x.Contains(y)))
                        .Select(x => x.RemoveInvalidDownloadmingSongsChars())
                        .ToList()
                }).Where(x => x.SongUrls.Count > 0 && LoadedFiles(urlString).All(y => x.AlbumName != y)).ToList();
                if (listMusicInfo.Count > 0) { 
                    Music.DoWrite(DirPath, listMusicInfo);
}
                Console.WriteLine(Resource.albumTaskComplete, Resource.donereadingUrl, urlString, listMusicInfo.Count);
            }
            catch (Exception) {
                Console.WriteLine(Resource.exceptionFor, urlString);
            }





        }







    }
}