using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class BusinessResult
    {
        public BusinessResult()
        {

        }
        public BusinessResult(bool succeeded, string error = "")
        {
            Succeeded = succeeded;
            Errors = [error];
        }

        public bool Succeeded { get; set; } = false;
        public List<string> Errors { get; set; } = [];
    }
}
