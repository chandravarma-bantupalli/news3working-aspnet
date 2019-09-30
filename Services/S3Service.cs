using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using SampleS3Upload.Models;

namespace SampleS3Upload.Services
{
  public class S3Service: IS3Service
  {
    private readonly IAmazonS3 _client;
    public S3Service(IAmazonS3 client)
    {
      _client = client;
    }

    public async Task<S3Response> CreateBucketAsync(string bucketName)
    {
      try
      {
        Console.WriteLine(bucketName);

        var putBucketRequest = new PutBucketRequest{
          BucketName = bucketName,
          UseClientRegion = true
        };

        var response = await _client.PutBucketAsync(putBucketRequest);
        Console.WriteLine(response);

        return new S3Response {
          StatusCode = response.HttpStatusCode,
          Message = response.ResponseMetadata.RequestId
        };
        
      } 
      catch (AmazonS3Exception aExe)
      {
        Console.WriteLine("In AmazonS3Exception");
        Console.WriteLine(aExe.Message);
        return new S3Response
        {
          StatusCode = aExe.StatusCode,
          Message = aExe.Message
        };
      } 
      catch (Exception exe)
      {
        Console.WriteLine("In Super Exception");
        Console.WriteLine(exe.Message);
        return new S3Response
        {
          StatusCode = System.Net.HttpStatusCode.InternalServerError,
          Message = exe.Message
        };
      }  
        
    }

    private const string filePath = "C:\\Users\\cgi\\Downloads\\dummy_1.pdf";
    private const string uploadWithKeyName = "UploadedWithKeyNameDummy";
    private const string fileStreamUpload = "FileStreamUpload";
    private const string AdvancedUpload = "AdvancedUpload";

    public async Task<string> UploadFileAsync(string bucketName)
    {
      try
      {
        var fileTransferUtility = new TransferUtility(_client);

        // option 1
        await fileTransferUtility.UploadAsync(filePath, bucketName);

        // option 2
        await fileTransferUtility.UploadAsync(filePath, bucketName, uploadWithKeyName);

        // option 3
        using ( var fileToUpload = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
          await fileTransferUtility.UploadAsync(fileToUpload, bucketName, fileStreamUpload);
        }

        // option 4
        var fileTransferUtilityRequest = new TransferUtilityUploadRequest{
          BucketName = bucketName,
          FilePath = filePath,
          StorageClass = S3StorageClass.Standard,
          PartSize = 6291456,
          CannedACL = S3CannedACL.NoACL
        };

        fileTransferUtilityRequest.Metadata.Add("param1", "value1");
        fileTransferUtilityRequest.Metadata.Add("param2", "value2");

        await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
        return (fileTransferUtilityRequest.ToString() + fileTransferUtility.ToString());
      }
      catch(AmazonS3Exception aExe)
      {
        Console.WriteLine(aExe.Message);
        return aExe.Message;
      }
      catch(Exception exe)
      {
        Console.WriteLine(exe.Message);
        return exe.Message;
      }
    }

    public async Task GetObjectFromS3Async(string bucketName)
    {
      const string keyName = "file.txt";

      try
      {
        var request = new GetObjectRequest{
          BucketName = bucketName,
          Key = keyName
        };
        string responseBody;
        
        using(var response = await _client.GetObjectAsync(request))
        using(var responseStream = response.ResponseStream)
        using(var reader = new StreamReader(responseStream))
        {
          var title = response.Metadata["x-amz-meta-title"];
          var contentType = response.Headers["Content-Type"];

          Console.WriteLine($"Object Meta, Title: {title}");
          Console.WriteLine($"Content Type: {contentType}");

          responseBody = reader.ReadToEnd();
        }

        var pathAndFileName = $"C:\\Users\\cgi\\Downloads\\s3files\\{keyName}";
        var createText = responseBody;

        File.WriteAllText(pathAndFileName, createText);
      }
      catch(AmazonS3Exception aExe)
      {
        Console.WriteLine(aExe.Message);
      }
      catch(Exception exe)
      {
        Console.WriteLine(exe.Message);
      }
    }
  }
}