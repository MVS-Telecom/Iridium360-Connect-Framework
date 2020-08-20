using System;


namespace Iridium360.Connect.Framework.Messaging
{
    public enum FileExtension : int
    {
        Jpg = 0,
    }

    public static class FileExtensionHelper
    {
        public static bool IsImage(this FileExtension ext)
        {
            return ext == FileExtension.Jpg;
        }
    }
}
