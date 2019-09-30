using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SampleS3Upload.Services;

namespace SampleS3Upload.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class S3BucketController: ControllerBase
  {
    private readonly IS3Service _service;
    public S3BucketController(IS3Service service)
    {
      _service = service;
    }
    [HttpPost]
    [Route("{bucketName}")]
    public async Task<IActionResult> CreateSubBucket([FromRoute] string bucketName)
    {
      Console.WriteLine(bucketName);
      var response = await _service.CreateBucketAsync(bucketName);
      return Ok(response);
    }
    [HttpPost]
    [Route("addfile/{bucketName}")]
    public async Task<IActionResult> AddFile([FromRoute] string bucketName)
    {
      var response = await _service.UploadFileAsync(bucketName);
      return Ok(response);
    }

    [HttpGet]
    [Route("getfile/{bucketName}")]
    public async Task<IActionResult> GetObjectFromS3Async([FromRoute] string bucketName)
    {
      await _service.GetObjectFromS3Async(bucketName);
      return Ok();
    }
  }
}