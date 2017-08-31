using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Type DType { get; set; }
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
                List<AutoConfigData> listAutoConfigData = new List<AutoConfigData>();
                foreach (var item in value)
                {
                    if (item.IndexOf(':') > 0)
                    {
                        listAutoConfigData.Add(new AutoConfigData { Key = item.Substring(1, item.IndexOf(':') - 1), Value = item.Substring(item.IndexOf(':') + 1).Split(',').ToList() });
                    }
                    else
                    {
                        listAutoConfigData.Add(new AutoConfigData { Key = item.Substring(1, 1) });
                    }
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
                        value = Convert.ToInt32(AutoConfigDataSet.Where(x => x.Type == autoConfigType).FirstOrDefault().Value.FirstOrDefault());
                    }
                    catch { }
                    return new AutoConfigValue { Value = value, DType = typeof(System.Int32) };

                case AutoConfigType.Url:
                    IEnumerable<string> urls = null;
                    try
                    {
                        urls = AutoConfigDataSet.Where(x => x.Type == autoConfigType).First().Value.ToList();
                    }
                    catch { }
                    return new AutoConfigValue { Value = urls, DType = typeof(List<string>) };
                case AutoConfigType.Auto:
                    if (AutoConfigDataSet.Where(x => x.Type == autoConfigType).Count() > 0)
                        return new AutoConfigValue { Value = true, DType = typeof(bool) };
                    else
                        return new AutoConfigValue { Value = false, DType = typeof(bool) };
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
                    default:
                        break;
                }
            }
        }
    }
}
