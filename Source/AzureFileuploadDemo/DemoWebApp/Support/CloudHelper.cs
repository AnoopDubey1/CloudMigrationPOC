using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;

namespace DemoWebApp.Support
{
    public class CloudHelper : ICloudService
    {
        public CloudFileDirectory GetFileShare(string dirname, bool createIfDoesntExist = false)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["storagekey"]);
            var fileClient = storageAccount.CreateCloudFileClient();
            var folderPath = dirname.Split('\\'); //ex: fileupload\uploads
            CloudFileShare share = fileClient.GetShareReference(folderPath[0]);
            CloudFileDirectory directory = null;
            if (!share.Exists())
            {
                throw new InvalidOperationException(string.Format("{0}share doesnt exists.", folderPath[0]));
            }

            directory = share.GetRootDirectoryReference();

            for (int i = 1; i < folderPath.Length && directory.Exists(); i++)
            {
                directory = directory.GetDirectoryReference(folderPath[i]);

                //Create if directory doesnt exists
                if (createIfDoesntExist && !directory.Exists())
                {
                    directory.Create();
                }
            }

            if (directory.Exists())
            {
                return directory;
            }

            throw new InvalidOperationException(string.Format("{0} directory doesnt exists.", directory.Name));
        }
    }

    public interface ICloudService
    {
        CloudFileDirectory GetFileShare(string dirname, bool createIfDoesntExist = false);
    }
}