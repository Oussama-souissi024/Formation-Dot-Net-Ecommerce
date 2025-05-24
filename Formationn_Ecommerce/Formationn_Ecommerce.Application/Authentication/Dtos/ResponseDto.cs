using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formationn_Ecommerce.Application.Authentication.Dtos
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Error { get; set; } = string.Empty;

        public ResponseDto() { }

        public ResponseDto(bool isSuccess, string error = "")
        {
            IsSuccess = isSuccess;
            Error = error;
        }
    }
}
