using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Queue; // Namespace for Queue storage types
using Newtonsoft.Json;

namespace MailQueueProcessorDemo
{
    class Program
    {
        public class MailContent
        {
            public string Name { get; set; }
            public string ContentBytes { get; set; }
            public string ContentType { get; set; }

        }

        public class MailQueueContent
        {
            public string messageid { get; set; }
            public string sender { get; set; }
            public string subject { get; set; }
            public int attachmentCount { get; set; }

        }

        static void Main(string[] args)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("storagekey"));

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudQueue queue = queueClient.GetQueueReference("pocmailrecords");
            var blobContainer = blobClient.GetContainerReference("pocmailstorage");

            // Create the queue if it doesn't already exist
            // queue.CreateIfNotExists();
            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for mails....");
                    // Peek at the next message
                    CloudQueueMessage queueMsg = queue.GetMessage(TimeSpan.FromMinutes(5));
                    if (queueMsg == null)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    var msgContent = JsonConvert.DeserializeObject<MailQueueContent>(queueMsg.AsString);

                    Console.WriteLine($"Obtained mail from {msgContent.sender} with subject:{msgContent.subject} Total Attachments:{msgContent.attachmentCount}");

                    for (int i = 0; i < msgContent.attachmentCount; i++)
                    {
                        var filenameBlob = blobContainer.GetBlobReference($"{msgContent.messageid}-{i}");
                        if (!filenameBlob.Exists())
                        {
                            continue;
                        }
                        using (var memStr = new MemoryStream())
                        {
                            filenameBlob.DownloadToStream(memStr);
                            memStr.Seek(0, SeekOrigin.Begin);

                            var str = new StreamReader(memStr);
                            var fileContent = JsonConvert.DeserializeObject<MailContent>(str.ReadToEnd());
                            var fileContentRaw = Encoding.UTF8.GetString(Convert.FromBase64String(fileContent.ContentBytes));
                            Console.WriteLine($"File content:{fileContent.Name}\r\n{fileContentRaw}");
                        }

                    }

                    queue.DeleteMessage(queueMsg);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }

            Console.ReadLine();
        }
    }
}
