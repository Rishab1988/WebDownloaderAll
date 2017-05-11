using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Music.Metadata.Bogie
{
    public class BogieStaticUrls
    {
        [ReadOnly(true)]
        private static string RootUrl
        {
            get
            {
                return "https://www.lyricsbogie.com/";
            }
        }
        [ReadOnly(true)]
        private static string[] AlbumCategoryUrl
        {
            get
            {
                return new string[] { RootUrl + "category/movies/", RootUrl + "category/albums/" };
            }
        }

        public static IEnumerable<AlbumCategoryUrls> AlbumCategoryIndexedUrl
        {
            get
            {
                
                List <AlbumCategoryUrls>  listAlbumCategoryUrls = new List<AlbumCategoryUrls> ();
                var listUrlStrings = new List<string>{AlbumCategoryUrl[0] + "#"};
                for (var c = 'a'; c <= 'z'; c++)
                {
                    listUrlStrings.Add(AlbumCategoryUrl[0] + c.ToString());
                }

                AlbumCategoryUrls albumCategoryUrls = new AlbumCategoryUrls {
                    Urls = listUrlStrings, Category = 0
                };


                listAlbumCategoryUrls.Add(albumCategoryUrls);

                listUrlStrings = new List<string> { AlbumCategoryUrl[1] + "#" };
                for (var c = 'a'; c <= 'z'; c++)
                {
                    listUrlStrings.Add(AlbumCategoryUrl[1] + c.ToString());
                }


                albumCategoryUrls = new AlbumCategoryUrls
                {
                    Urls = listUrlStrings,
                    Category = 1
                };

                listAlbumCategoryUrls.Add(albumCategoryUrls);


                return listAlbumCategoryUrls;

            }
        }
    }

    public class BogieSearchQuries
    {
        public static string AlbumSearchQuery
        {
            get
            {
                return "ul.cat_list li a";
            }
        }

        public static string SongsSearchQuery
        {
            get
            {
                return "ul.song_list li h3 a";
            }
        }
        public static string SongsLyricsSearchQuery
        {
            get
            {
                return "#lyricsDiv";
            }
        }
    }


    public class Album
    {
        public string AlbumName { get; set; }
        public string AlbumUrl { get; set; }
    }

    public class Song
    {
        public string SongName { get; set; }
        public string SongUrl { get; set; }
    }

    public class AlbumCategoryUrls
    {
        public List<string> Urls { get; set; }
        public int Category { get; set; }
    }

    public class Bogie : IWebDownload
    {
        public void DoStackCall()
        {
            DoSearchCall();
        }


        //private static string DirPath
        //{
        //    get { return @"D:\Rishab\Task2\lyricsbogie.com\"; }
        //}

        private static string DirPath(int category)
        {

            if (category == 0)
                return @"D:\Rishab\Task2\lyricsbogie.com\";
            else if (category == 1)
                return @"D:\Rishab\Task2\lyricsbogie.com\Album\";
            else
                throw new Exception();
        }

        private static List<string> LoadedFiles(string dirPath)
        {
            if (!Directory.Exists(dirPath))
                return new List<string>();

            return Directory.GetFiles(dirPath, "*.txt", SearchOption.TopDirectoryOnly).Select(Path.GetFileNameWithoutExtension)
                .ToList();
        }

        private void DoSearchCall()
        {
            //Console.Clear();
            foreach (var indexedUrl in BogieStaticUrls.AlbumCategoryIndexedUrl)
            {
                foreach (var url in indexedUrl.Urls)
                {
                    DoSearch(url, indexedUrl.Category);
                    //Task.Run(() => DoSearch(url, 0));
                }

              
            }

            //DoSearch("https://www.lyricsbogie.com/category/movies/p",0);
        }

        HtmlNode GetPage(string indexedUrl)
        {
            var web = new HtmlWeb();
            return web.Load(indexedUrl).DocumentNode;
        }

        IEnumerable<Album> GetAlbumList(string indexedUrl)
        {
            return GetPage(indexedUrl).QuerySelectorAll(BogieSearchQuries.AlbumSearchQuery)
                 .Select(x => new Album
                 {
                     AlbumName = x.InnerText.RemoveInvalidPathChars(),
                     AlbumUrl = x.GetAttributeValue("href", "")
                 });
        }

        IEnumerable<Song> GetSongsList(string albumUrl)
        {
            return GetPage(albumUrl).QuerySelectorAll(BogieSearchQuries.SongsSearchQuery).Select(x => new Song
             {
                 SongName = x.InnerText.RemoveInvalidPathChars(),
                 SongUrl = x.GetAttributeValue("href", "")
             });
        }

        SongInfo GetSongData(Song song)
        {
            var page = GetPage(song.SongUrl);
            var lyricsContainer = page.QuerySelector(BogieSearchQuries.SongsLyricsSearchQuery);
            string lyrics = null;
            if (lyricsContainer != null)
                lyrics = lyricsContainer.InnerHtml.ReplaceBogieLyricsChars();
            var songData = page.QuerySelectorAll("p").Where(x => x.QuerySelectorAll("span.title").Count() > 0 && x.InnerText.Length > 0).Select(y => y.InnerText).Select(z => z.Substring(z.IndexOf(':') + 2)).Skip(1).ToArray();

            if (songData.Length > 0)
            {
                SongInfo songInfo = new SongInfo
                {
                    SongName = song.SongName.RemoveInvalidPathChars(),
                    Artist = new Artist { Value = songData[0] },
                    Writer = new Writer { Value = songData[1] },
                    Composer = new Composer { Value = songData[2] },
                    Release = new Release { Value = songData[songData.Length - 1] },
                    Lyrics = new Lyrics { Value = lyrics }
                };

                return songInfo;
            }
            else
            {
                return new SongInfo();
            }
        }

        private void DoSearch(string indexedUrl, int category)
        {
            //Console.WriteLine(indexedUrl);
            var albumList = GetAlbumList(indexedUrl);

            foreach (var album in albumList)
            {
                var pDirPath = DirPath(category) + album.AlbumName;

                if (!Directory.Exists(pDirPath) || album.AlbumName.IndexOf("(2017)") > 0)
                {

                    var albumInfo = new AlbumInfo { AlbumName = album.AlbumName, SongInfo = new List<SongInfo>() };
                    var songsList = GetSongsList(album.AlbumUrl);
                    foreach (var song in songsList)
                    {
                        var songData = GetSongData(song);
                        if (songData == null)
                        {
                            FileInfo file = new FileInfo(DirPath(category) + "noData.txt");
                            StreamWriter writer = file.AppendText();
                            writer.WriteLine(album.AlbumName);
                            writer.Write("\t");
                            writer.Write(song.SongName);
                            writer.Close();
                        }
                        else
                            albumInfo.SongInfo.Add(GetSongData(song));


                    }
                    try
                    {
                        WriteMetadata(albumInfo, category);
                    }
                    catch { 
                    }
                }
            }

            Console.WriteLine("Completed: " + indexedUrl);
        }

        private void WriteMetadata(AlbumInfo albumInfo, int category)
        {
            //Console.WriteLine(albumInfo.AlbumName);
            var pDirPath = DirPath(category) + albumInfo.AlbumName;



            //if (!Directory.Exists(pDirPath))
            //{
                Directory.CreateDirectory(pDirPath);



                foreach (var songInfo in albumInfo.SongInfo)
                {
                    var filePath = pDirPath + "\\" + songInfo.SongName + ".txt";
                    if (File.Exists(filePath))
                        continue;
                    File.WriteAllText(filePath, songInfo.ToString());
                }
            //}

        }
    }
}
