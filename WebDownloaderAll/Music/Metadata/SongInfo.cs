using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace WebDownloaderAll.Music.Metadata
{
    public class Artist
    {
        public static string Name
        {
            get { return "Artist: "; }
        }
        public string Value { get; set; }
    }
    public class Composer
    {
        public static string Name
        {
            get { return "Composer: "; }
        }
        public string Value { get; set; }
    }
    public class Writer
    {
        public static string Name
        {
            get { return "Lyricist: "; }
        }
        public string Value { get; set; }
    }
    public class Release
    {
        public static string Name
        {
            get { return "Release on: "; }
        }
        public string Value { get; set; }
    }
    public class Lyrics
    {
        public static string Name
        {
            get { return "---------Lyrics----------"; }
        }
        public string Value { get; set; }
    }
    public class SongInfo
    {
        public SongInfo() { }

        [Obsolete("Try putting these values in AlbumInfo")]
        public SongInfo(string dirPath, string albumName)
        {
            DirPath = dirPath;
            AlbumName = albumName;
        }
        [Obsolete("Try putting these values in AlbumInfo")]
        private string AlbumName { get; set; }
        [Obsolete("Try putting these values in AlbumInfo")]
        private string DirPath { get; set; }

        public string SongName { get; set; }
        public Artist Artist { private get; set; }
        public Composer Composer { private get; set; }
        public Writer Writer { private get; set; }
        public Lyrics Lyrics { private get; set; }
        public Release Release { private get; set; }


        public string SongDirPath
        {
            get { return DirPath + AlbumName; }
        }


        public string Path
        {
            get
            {
                return DirPath + AlbumName + "\\" + SongName + ".txt";
            }
        }

        public override string ToString()
        {
            var songInfoString = new StringBuilder();
            songInfoString.Append(Artist.Name)
                .Append(Artist.Value)
                .Append(Environment.NewLine)
                .Append(Composer.Name)
                .Append(Composer.Value)
                .Append(Environment.NewLine)
                .Append(Writer.Name)
                .Append(Writer.Value)
                .Append(Environment.NewLine);

            if (!String.IsNullOrEmpty(Release.Value))
                songInfoString.Append(Release.Name).Append(Release.Value).Append(Environment.NewLine);

            songInfoString.Append(Environment.NewLine);


            if (Lyrics != null)
            {
                songInfoString.Append(Lyrics.Name)
                .Append(Environment.NewLine)
                .Append(Lyrics.Value ?? "");
            }

            return songInfoString.ToString();
        }
    }

    public class AlbumInfo
    {
        public string AlbumName { get; set; }
        public List<SongInfo> SongInfo { get; set; }
    }

    internal class SongMetadataException : Exception
    {
        public SongMetadataException(string message)
            : base(message)
        {
        }


        // ReSharper disable once UnusedMember.Global
        public SongMetadataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
