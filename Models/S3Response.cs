using System;
using System.Net;

namespace SampleS3Upload.Models
{
  public class S3Response
  {
    public HttpStatusCode StatusCode { get; set; }
    public string Message { get; set; }
  }
}