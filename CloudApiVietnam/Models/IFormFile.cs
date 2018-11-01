using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CloudApiVietnam.Models
{
    public interface IFormFile
    {
        string ContentType { get; }
        string ContentDisposition { get; }
        IHeaderDictionary Headers { get; }
        long Length { get; }
        string Name { get; }
        string FileName { get; }
        Stream OpenReadStream();
        void CopyTo(Stream target);
        //Task CopyToAsync(Stream target, CancellationToken cancellationToken = null);
    }


}