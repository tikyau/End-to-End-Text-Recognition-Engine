using System;
using System.IO;

namespace AzureCustomVisionService
{
    class Util
    {
        /// <summary>
        /// Convert image to byte array
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <returns>image data</returns>
        public static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }

}
