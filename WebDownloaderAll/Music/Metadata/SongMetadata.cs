using System.Collections.Generic;
using System.ComponentModel;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Music.Metadata
{
    public abstract class SongMetadata : IWebDownload
    {
        public void DoStackCall()
        {

        }

        [ReadOnly(true)]
        protected abstract string DirPath { get; }

        [ReadOnly(true)]
        protected abstract string RootUrl { get; }

        [ReadOnly(true)]
        protected virtual IEnumerable<string> UrlStrings
        {
            get
            {
                var listUrlStrings = new List<string>();
                for (var c = 'a'; c <= 'z'; c++)
                {
                    listUrlStrings.Add(c.ToString());
                }
                return listUrlStrings;
            }
            

        }

        [ReadOnly(true)]
        protected abstract IEnumerable<string> LoadedFiles(string urlString);

        protected abstract void DoSearchcall(IEnumerable<string> urlStrings);

        protected abstract void DoSearch();


    }
}