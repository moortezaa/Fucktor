using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class BusinessResult
    {
        public bool Succeeded { get; set; } = false;
        public List<string> Errors { get; set; } = [];
    }
}
