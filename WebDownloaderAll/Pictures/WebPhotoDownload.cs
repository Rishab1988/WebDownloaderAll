using System;
using System.Collections.Generic;
using System.ComponentModel;
using WebDownloaderAll.Common;

namespace WebDownloaderAll.Pictures
{
    public abstract class WebPhotoDownload : IWebDownload
    {
        public virtual void DoStackCall()
        {
            var userInput = Photo.TakeUserInput();
            var userInputCount = userInput.Count;
            for (int i = 0; i < userInputCount; i++)
            {
                try
                {
                    Console.Clear();
                    Console.Write(Resource.readingUrlFrom, i + 1, userInputCount);
                    Photo.DoDownload(DoImageDownload(userInput[i]));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
            Console.WriteLine();
        }

        [ReadOnly(true)]
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        protected virtual string PhotoPath
        {
            get { return @"D:\Rishab\Do Copy\Images\"; }
        }

        protected abstract List<PhotoDownload> DoImageDownload(PhotoInput photoInput);
    }
}