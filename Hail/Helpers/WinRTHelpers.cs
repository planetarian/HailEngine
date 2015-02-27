using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

#if WINRT

namespace System.IO
{
    public static class File
    {
        public static string ReadAllText(string path)
        {
            StorageFolder installFolder = Package.Current.InstalledLocation;
            Task<StorageFile> fileTask = installFolder.GetFileAsync(path).AsTask();
            fileTask.Wait();
            StorageFile file = fileTask.Result;
            Task<string> stringTask = FileIO.ReadTextAsync(file).AsTask();
            stringTask.Wait();
            return stringTask.Result;
        }
    }
}

namespace Hail.Helpers
{
    public static class TypeExtensions
    {
    }
}

#endif