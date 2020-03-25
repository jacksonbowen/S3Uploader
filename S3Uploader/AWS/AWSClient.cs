using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace S3Uploader.AWS
{
  public class AWSClient
    : IDisposable
  {
    private readonly string _bucketName;
    private readonly RegionEndpoint _bucketRegion = RegionEndpoint.USEast1;
    private readonly IAmazonS3 _s3Client;


    public AWSClient(
      string bucketName)
    {
      _bucketName = bucketName;
      _s3Client = new AmazonS3Client(_bucketRegion);
    }


    public async Task UploadFileAsync(
      FileInfo fileInfo)
    {
      try
      {
        var fileNameWithoutExtension = fileInfo.Name.Substring(
          0, fileInfo.Name.LastIndexOf('.'));

        var fileExtension = fileInfo.Extension;

        var keyName = $"{fileNameWithoutExtension}" +
          $"-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}{fileExtension}";

        var fileTransferUtility = new TransferUtility(_s3Client);

        await fileTransferUtility
          .UploadAsync(
            fileInfo.FullName, 
            _bucketName,
            keyName);
      }
      catch (AmazonS3Exception ex)
      {
        throw new Exception(
          $"Error encountered on server. '{ex}' when writing an object.");
      }
      catch (Exception ex)
      {
        throw new Exception(
          $"Unknown encountered on server. '{ex}' when writing an object");
      }
    }

    public void Dispose()
    {
      _s3Client?.Dispose();
    }
  }
}



/*				Console.WriteLine("Upload 1 completed");
        // Option 2. Specify object key name explicitly.
        await fileTransferUtility.UploadAsync(filePath, bucketName, keyName);
        Console.WriteLine("Upload 2 completed");
        // Option 3. Upload data from a type of System.IO.Stream.
        using (var fileToUpload =
            new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
          await fileTransferUtility.UploadAsync(fileToUpload,
                                     bucketName, keyName);
        }
        Console.WriteLine("Upload 3 completed");
        // Option 4. Specify advanced settings.
        var fileTransferUtilityRequest = new TransferUtilityUploadRequest
        {
          BucketName = bucketName,
          FilePath = filePath,
          StorageClass = S3StorageClass.StandardInfrequentAccess,
          PartSize = 6291456, // 6 MB.
          Key = keyName,
          CannedACL = S3CannedACL.PublicRead
        };
        fileTransferUtilityRequest.Metadata.Add("param1", "Value1");
        fileTransferUtilityRequest.Metadata.Add("param2", "Value2");
        await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
        Console.WriteLine("Upload 4 completed");*/
