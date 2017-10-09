using System;
using System.Collections.Generic;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Music.Metadata
{
    public class GeetMalaUrl
    {
        public GeetMalaUrl(string urlString) {

            UrlString = urlString;

            var web = new HtmlWeb();
            var tempmaxIndex = web.Load(FormattedUrl(1)).DocumentNode.QuerySelector(MaxIndexSearchQuery).InnerText;
            MaxPageIndex = Convert.ToInt32(tempmaxIndex.Substring(tempmaxIndex.Length - 2));
        }


        private string UrlString { get; set; }

        private static string RootUrl {
            get {
                return "http://www.hindigeetmala.net/movie/";
            }
        }

        public static string CoreUrl {
            get {
                return "http://www.hindigeetmala.net";
            }
        }

        private static string MaxIndexSearchQuery {
            get {
                return "td.alcen.w720.bg7f";
            }
        }

        public static string AlbumSearchQuery {
            get {
                return "td.w25p.h150 a";
            }
        }

        public static string SongSearchQuery {
            get {
                return "td.w185 a";
            }
        }

        public static string SongTitleSearchQuery {
            get {
                return "table.b1.w760.pad2.allef td.w115";
            }
        }

        public static string SongInfoSearchQuery {
            get {
                return "table.b1.w760.pad2.allef td.w110";
            }
        }

        public static string LyricsSearchQuery {
            get { return "div.song"; }
        }

        public static IEnumerable<string> UrlStrings {
            get {
                var listUrlStrings = new List<string> { "0-9" };
                for (var c = 'a'; c <= 'z'; c++) {
                    listUrlStrings.Add(c.ToString());
                }
                return listUrlStrings;
            }
        }


        private string FormattedUrl(int pageIndex) {
            return string.Format("{0}{1}.php?page={2}", RootUrl, UrlString, pageIndex);
        }

        // ReSharper disable once UnusedMember.Global
        public string RootFormattedUrl {
            get { return string.Format("{0}{1}.php", RootUrl, UrlString); }
        }

        public string CurrentFormattedUrl {
            get {
                return FormattedUrl(CurrentPageIndex);
            }

        }


        private int MaxPageIndex { get; set; }

        private int CurrentPageIndex { get; set; }

        public bool CanMoveNext {
            get { return CurrentPageIndex < MaxPageIndex; }
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private string MoveNext() {
            if (CurrentPageIndex >= MaxPageIndex)
                throw new SongMetadataException(Resource.collectioniterationComplete);
            CurrentPageIndex++;
            return FormattedUrl(CurrentPageIndex);
        }
        public GeetMalaUrl MoveNextStep()
        {
            MoveNext();
            return this;
        }

    }
}