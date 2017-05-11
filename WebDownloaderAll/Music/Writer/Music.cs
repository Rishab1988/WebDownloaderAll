using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace WebDownloaderAll.Music.Writer
{
    public class MusicInfo
    {
        public string AlbumName { set; get; }
        //public string AlbumUrl { set; get; }
        [ReadOnly(true)]
        public string FileName {
            get {
                return "\\" + AlbumName + ".txt";
            }
        }
        public List<string> SongUrls { get; set; }
    }

    internal class MusicException : Exception
    {
        public MusicException(string message)
            : base(message) {
        }

        public MusicException(string message, Exception innerException)
            : base(message, innerException) {
        }
    }


    public static class Music
    {
        public static void DoWrite(string directory, List<MusicInfo> listMusicDownload) {
            if (listMusicDownload == null)
                throw new MusicException("No information found to write");
            if (listMusicDownload.Count == 0)
                throw new MusicException("No information found to write");

            var directoryInfo = new DirectoryInfo(directory);
            directoryInfo.Create();

            foreach (var musicDownload in listMusicDownload) {
                string filePath;
                bool hasTempPath = musicDownload.SongUrls.Any(x => x.IndexOf("temp", StringComparison.OrdinalIgnoreCase) > 0);

                if (hasTempPath)
                    filePath = directory + "\\temp" + musicDownload.FileName;

                else
                    filePath = directory + musicDownload.FileName;

                if (File.Exists(filePath)) {
                    if (!hasTempPath)
                        continue;

                    var existingLength = File.ReadAllLines(filePath).Count(x => x.Length > 0);
                    if (existingLength == musicDownload.SongUrls.Count)
                        continue;

                    File.Delete(filePath);
                }


                File.WriteAllLines(filePath, musicDownload.SongUrls);
                
            }
        }


    }
}
