using HelpModel.CustomAttribute;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HelpModel.DTO.HelpDesk
{
    public class TblHelpDeskDTO_AM
    {
        public TblHelpDesk_AM HelpDesk{ get; set; }
        public List<TblHelpDeskHistories_AM> HelpDeskRemarks{ get; set; }
        public List<TblHelpDeskDocuments_AM> HelpDeskFiles{ get; set; }
        public List<PriorityType_AM> FlagTypeList{ get; set; }
        public List<TblHelpDeskComplaintType_AM> ComplaintTypeList { get; set; }


    }
    
}
