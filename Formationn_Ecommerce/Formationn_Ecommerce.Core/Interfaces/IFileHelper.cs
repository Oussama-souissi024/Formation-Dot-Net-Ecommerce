using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Formationn_Ecommerce.Core.Interfaces
{
    public interface IFileHelper
    {
        // Uploads a file to the specified folder
        // Returns the URL/path of the uploaded file, or null if upload fails
        // folder parameter specifies the destination directory
        public string? UploadFile(IFormFile file, string folder);

        // Deletes a file from the specified folder
        // Returns true if deletion successful, false otherwise
        // imagePath: path to the file, Folfer: directory containing the file
        public bool DeleteFile(string Path, string folder);

    }
}
