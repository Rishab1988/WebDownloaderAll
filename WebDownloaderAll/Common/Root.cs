using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WebDownloaderAll.FromFile;
using WebDownloaderAll.Music.Metadata;
using WebDownloaderAll.Music.Metadata.Bogie;
using WebDownloaderAll.Music.Writer;
using WebDownloaderAll.Pictures;

namespace WebDownloaderAll.Common
{
    public class DownloadOption
    {
        public int Value { get; set; }
        public string Text { private get; set; }
        public bool IsLong { get; set; }

        public IWebDownload Download { get; set; }

        [ReadOnly(true)]
        public string DisplayText
        {
            get { return string.Format("{0} : {1}", Value, Text); }
        }
    }




    public class DownloadOptionException : Exception
    {
        public DownloadOptionException()
        {
        }
        public DownloadOptionException(string message)
            : base(message)
        {
        }
    }


    internal class CSystem : IWebDownload
    {

        public void DoStackCall()
        {
            Environment.Exit(0);
        }


       
    }

    public class Root
    {
        public bool DoStackCall()
        {
            int? choice = AutoConfigProvider.GetValue(AutoConfigType.Choice).Value;
            var data = PrepareOptions();
            PrintOptions(data);
            return UserSelect(data, choice);
        }

        private static List<DownloadOption> PrepareOptions()
        {
            var listDownloadOption = new List<DownloadOption>();

            var downloadOption = new DownloadOption
            {
                Value = 0,
                Text = "Exit..",
                Download = new CSystem()
            };
            listDownloadOption.Add(downloadOption);

            downloadOption = new DownloadOption
            {
                Value = 1,
                Text = Resource.sputnikPhoto,
                Download = new SputnikPhoto()
            };
            listDownloadOption.Add(downloadOption);

            downloadOption = new DownloadOption
            {
                Value = 2,
                Text = Resource.rbthPhoto,
                Download = new RbthPhoto()
            };
            listDownloadOption.Add(downloadOption);

            downloadOption = new DownloadOption
            {
                Value = 3,
                Text = Resource.downloadming,
                Download = new Downloadming(),
                IsLong = true
            };
            listDownloadOption.Add(downloadOption);

            downloadOption = new DownloadOption
            {
                Value = 5,
                Text = Resource.geetMala,
                Download = new GeetMala(),
                IsLong = false
            };
            listDownloadOption.Add(downloadOption);

            downloadOption = new DownloadOption
            {
                Value = 6,
                Text = Resource.lyricsBogie,
                Download = new Bogie(),
                IsLong = false
            };
            listDownloadOption.Add(downloadOption);


            downloadOption = new DownloadOption
            {
                Value = 7,
                Text = Resource.fromFileDownload,
                Download = new FromFileDownloader(),
                IsLong = false
            };
            listDownloadOption.Add(downloadOption);

            downloadOption = new DownloadOption
            {
                Value = 8,
                Text = Resource.rtvision,
                Download = new RtVision()
            };
            listDownloadOption.Add(downloadOption);

            if (listDownloadOption.GroupBy(x => x.Value).Any(y => y.Count() > 1))
                throw new DownloadOptionException(Resource.duplicateValue);

            return listDownloadOption;
        }

        private static void PrintOptions(IEnumerable<DownloadOption> listOptions)
        {
            Console.WriteLine(Resource.selectOptions);
            foreach (var option in listOptions)
            {
                Console.WriteLine(option.DisplayText);
            }
        }

        private static bool UserSelect(IEnumerable<DownloadOption> listOptions, int? autoChoice = null)
        {
            int choice;

            if (autoChoice.HasValue)
                choice = autoChoice.Value;
            else
            {
                while (true)
                {
                    Console.WriteLine(Resource.userChoice);
                    if (int.TryParse(Console.ReadLine(), out choice))
                        break;
                }
            }

            var webdownload = listOptions.First(x => x.Value == choice);
            Console.WriteLine(Resource.workingWaitKey);
            webdownload.Download.DoStackCall();
            return !webdownload.IsLong;
        }
    }

}
