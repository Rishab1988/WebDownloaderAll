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

            List<PhotoInput> userInput;
            userInput = Convert.ToBoolean(AutoConfigProvider.GetValue(AutoConfigType.Auto).Value) ? Photo.TakeUserInput(AutoUrl) : Photo.TakeUserInput();
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

        protected virtual string AutoUrl
        {
            get
            {
                throw new NotImplementedException("Please override with your functionality");
            }
        }

        protected abstract List<PhotoDownload> DoImageDownload(PhotoInput photoInput);



    }
}