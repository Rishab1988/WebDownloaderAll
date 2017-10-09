using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Music.Metadata
{
    public class GeetMala : IWebDownload
    {
        private static string DirPath {
            get { return @"D:\Rishab\Task2\hindigeetmala.net\"; }
        }

        public void DoStackCall() {
            DoSearchCall();
        }

        private static List<string> LoadedFiles(string dirPath) {
            if (!Directory.Exists(dirPath))
                return new List<string>();

            return Directory.GetFiles(dirPath, "*.txt", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        // ReSharper disable once UnusedMember.Local
        private static void WriteMetadata(IEnumerable<AlbumInfo> albumInfo) {

            foreach (var album in albumInfo) {

                var proceesedDirPath = Path.GetDirectoryName(album.SongInfo.First().Path);

                if (proceesedDirPath == null)
                    throw new ArgumentException();

                if (!Directory.Exists(proceesedDirPath))
                    Directory.CreateDirectory(proceesedDirPath);

                foreach (var songInfo in album.SongInfo) {
                    var filePath = songInfo.Path;
                    if (File.Exists(filePath))
                        continue;
                    File.WriteAllText(filePath, songInfo.ToString());
                }

            }
        }

        private static void WriteMetadata(AlbumInfo album) {

            var proceesedDirPath = album.SongInfo.First().SongDirPath;

            if (proceesedDirPath == null)
                throw new ArgumentException();

            if (!Directory.Exists(proceesedDirPath))
                Directory.CreateDirectory(proceesedDirPath);

            foreach (var songInfo in album.SongInfo) {
                var filePath = songInfo.Path;
                if (File.Exists(filePath))
                    continue;
                File.WriteAllText(filePath, songInfo.ToString());
            }

        }

        private static void DoSearchCall() {
            foreach (var urlString in GeetMalaUrl.UrlStrings) {
                var geetMalaUrl = new GeetMalaUrl(urlString);
                Console.Clear();
                DoSearchThreadedCall(geetMalaUrl, true);
            }
        }

        private static void DoSearchThreadedCall(GeetMalaUrl geetMalaUrl, bool showMessages) {
            while (geetMalaUrl.CanMoveNext) {
                DoSearch(geetMalaUrl.MoveNextStep(), showMessages);
            }
        }

        private static void DoSearch(GeetMalaUrl geetMalaUrl, bool showMessages) {
            Console.Clear();
            if (showMessages)
                Console.WriteLine(Resource.openingUrl, geetMalaUrl.CurrentFormattedUrl);

            var web = new HtmlWeb();
            var page =
                web.Load(geetMalaUrl.CurrentFormattedUrl)
                    .DocumentNode.QuerySelectorAll(GeetMalaUrl.AlbumSearchQuery)
                    .Where(x => x.InnerText.Trim().Length > 0);



            foreach (var album in page) {

                var albuminfo = new AlbumInfo
                {
                    AlbumName = album.InnerText.RemoveInvalidPathChars(),
                    SongInfo = new List<SongInfo>()
                };


                if (showMessages)
                    Console.WriteLine(Resource.openingAlbumNameParam, albuminfo.AlbumName);

                var albumInfoData = web.Load(GeetMalaUrl.CoreUrl + album.GetAttributeValue("href", ""))
                    .DocumentNode.QuerySelectorAll(GeetMalaUrl.SongSearchQuery).Select(x => x.GetAttributeValue("href", ""));

                foreach (var xalbumInfo in albumInfoData) {
                    var songInfoComplexData = web.Load(GeetMalaUrl.CoreUrl + xalbumInfo)
                        .DocumentNode;
                    var songInfoData = songInfoComplexData.QuerySelectorAll(GeetMalaUrl.SongInfoSearchQuery).ToList();


#pragma warning disable 618
                    var songInfo = new SongInfo(DirPath, albuminfo.AlbumName)
#pragma warning restore 618
                    {
                        Artist = new Artist { Value = songInfoData.ElementAt(0).InnerText },
                        Composer = new Composer { Value = songInfoData.ElementAt(1).InnerText },
                        Writer = new Writer { Value = songInfoData.ElementAt(2).InnerText },
                        SongName = songInfoComplexData.QuerySelector(GeetMalaUrl.SongTitleSearchQuery)
                            .InnerText.RemoveInvalidPathChars()
                    };



                    var lyrics = songInfoComplexData.QuerySelector(GeetMalaUrl.LyricsSearchQuery);
                    if (lyrics != null)
                        songInfo.Lyrics = new Lyrics
                        {
                            Value = lyrics.InnerText
                        };

                    if (LoadedFiles(songInfo.SongDirPath).Contains(songInfo.SongName)) continue;
                    albuminfo.SongInfo.Add(songInfo);
                }
                if (albuminfo.SongInfo.Count > 0)
                    WriteMetadata(albuminfo);
            }
            if (showMessages)
                Console.WriteLine(Resource.ClosingUrl, geetMalaUrl.CurrentFormattedUrl);
        }

    }
}
