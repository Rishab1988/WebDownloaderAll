using System;
using System.IO;
using System.Linq;

namespace WebDownloaderAll.Common
{
    public static class Utility
    {
        public static string RemoveInvalidPathChars(this string filePath) {
            return Path.GetInvalidFileNameChars().Aggregate(filePath, (current, ic) => current.Replace(ic, '-'));
        }

        public static string RemoveInvalidDownloadmingAlbumChars(this string album)
        {
            string newAlbum;
            if (album.IndexOf("pop", StringComparison.OrdinalIgnoreCase) > 0)
            {
                newAlbum = album.Substring(0, album.LastIndexOf("-", StringComparison.Ordinal));
                if (album.IndexOf("- Punjabi", StringComparison.Ordinal) > 0)
                    newAlbum = newAlbum + " (Punjabi)";
                else
                    newAlbum = newAlbum + " (Album)";
            }
            else if (album.IndexOf("punjabi", StringComparison.OrdinalIgnoreCase) > 0)
            {
                newAlbum = album.Substring(0, album.LastIndexOf("-", StringComparison.Ordinal)) + " (Punjabi)";
            }
            else
            {
                newAlbum = album.Replace("- MP3 Songs","");
                newAlbum = newAlbum.Replace("-MP3 Songs", "");
            }
            return newAlbum;
        }

        public static string RemoveInvalidSputnikPhotoChars(this string photoPath)
        {
            photoPath = photoPath.Replace(" - Sputnik International", "");
            return photoPath;
        }

        public static string RemoveInvalidDownloadmingSongsChars(this string album) {
            album = album.Replace("&amp;", "&");
            return album;
        }

         public static string ReplaceBogieLyricsChars(this string lyrics)
        {
            return lyrics.Replace("<blockquote>", "").Replace("</blockquote>", "").Replace("<p>", "").Replace("</p>", "\r\n\r\n").Replace("<br>", "\r\n").Trim();
        }

         public static string GetNameFromUrl(this string urlString)
         {
             var uri = new Uri(urlString);
             var name = Path.GetFileName(uri.LocalPath);
             return name;
         }
    }
}
