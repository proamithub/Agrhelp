using HelpModel.CustomAttribute;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HelpModel.DTO.HelpDesk
{
    public class TblHelpDeskComplaintType_AM
    {
        public int CTypeID { get; set; }
        public string? TypeName { get; set; }
        public string? Details { get; set; }       
        public bool? IsActive { get; set; }
        public bool? ForAdmin { get; set; }
        public int? Sequence { get; set; }
    }
}
