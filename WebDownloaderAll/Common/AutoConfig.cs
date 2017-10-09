using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WebDownloaderAll.Common
{
    public enum AutoConfigType
    {
        Choice,
        Url,
        Auto
    }

    public class AutoConfigValue
    {
        public dynamic Value { get; set; }
        public Type DType { [UsedImplicitly] get; set; }
    }

    public class AutoConfigData
    {
        public string Key { get; set; }
        public AutoConfigType? Type { get; set; }
        public IEnumerable<string> Value { get; set; }
    }

    public static class AutoConfigProvider
    {

        public static IEnumerable<AutoConfigData> AutoConfigDataSet { get; set; }

        public static IEnumerable<string> ProcessConfig
        {
            set
            {
                var listAutoConfigData = new List<AutoConfigData>();
                foreach (var item in value)
                {
                    listAutoConfigData.Add(item.IndexOf(':') > 0
                        ? new AutoConfigData
                        {
                            Key = item.Substring(1, item.IndexOf(':') - 1),
                            Value = item.Substring(item.IndexOf(':') + 1).Split(',').ToList()
                        }
                        : new AutoConfigData {Key = item.Substring(1, 1)});
                }
                AutoConfigDataSet = listAutoConfigData;

                //AutoConfigDataSet = value.Where(y => y.IndexOf(':') > 0).Select(x => new AutoConfigData { Key = x.Substring(1, x.IndexOf(':') - 1), Value = x.Substring(x.IndexOf(':') + 1).Split(',').ToList() }).ToList();
                AutoConfigParser();
            }
        }

        public static AutoConfigValue GetValue(AutoConfigType autoConfigType)
        {
            switch (autoConfigType)
            {
                case AutoConfigType.Choice:
                    int? value = null;

                    try
                    {
                        // ReSharper disable once PossibleNullReferenceException
                        value = Convert.ToInt32(AutoConfigDataSet.FirstOrDefault(x => x.Type == autoConfigType)
                            .Value.FirstOrDefault());
                    }
                    catch
                    {
                        // ignored
                    }
                    return new AutoConfigValue { Value = value, DType = typeof(int) };

                case AutoConfigType.Url:
                    IEnumerable<string> urls = null;
                    try
                    {
                        urls = AutoConfigDataSet.First(x => x.Type == autoConfigType).Value.ToList();
                    }
                    catch
                    {
                        // ignored
                    }
                    return new AutoConfigValue { Value = urls, DType = typeof(List<string>) };
                case AutoConfigType.Auto:
                    return AutoConfigDataSet.Any(x => x.Type == autoConfigType) ? new AutoConfigValue { Value = true, DType = typeof(bool) } : new AutoConfigValue { Value = false, DType = typeof(bool) };
                default:
                    throw new InvalidOperationException(Resource.NoAutoConfigData);

            }
        }





        private static void AutoConfigParser()
        {
            foreach (var autoConfigData in AutoConfigDataSet)
            {
                switch (autoConfigData.Key.ToUpper())
                {
                    case "C":
                        autoConfigData.Type = AutoConfigType.Choice;
                        break;
                    case "U":
                        autoConfigData.Type = AutoConfigType.Url;
                        break;
                    case "A":
                        autoConfigData.Type = AutoConfigType.Auto;
                        break;
                }
            }
        }
    }
}
