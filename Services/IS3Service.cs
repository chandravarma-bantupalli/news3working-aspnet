using System;
using System.Threading.Tasks;
using SampleS3Upload.Models;

namespace SampleS3Upload.Services
{
  public interface IS3Service
  {
      Task<S3Response> CreateBucketAsync(string bucketName);
      Task<string> UploadFileAsync(string bucketName);
      Task GetObjectFromS3Async(string bucketName);
  }
}