using How.Common.Attributes;
using How.Common.Enums;
using Microsoft.AspNetCore.Http;

namespace How.Core.DTO.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public sealed class CreateImageRequestDTO
    {       
        [FileValidator(new AppFileExt[] { AppFileExt.JPEG }, 10 * 1024 * 1024  )]
        public IFormFile File { get; set; }
    }
}
