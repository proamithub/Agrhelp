using HelpModel.DTO.HelpDesk;
using HelpModel.DTO.Services;
using HelpRepo;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;


namespace HelpUI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    
    public class ApiController : BaseApiController
    {

        int pageSize;

        public ApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            pageSize = 0;//WebConfigSetting.PageSize;
        }

        [HttpGet]
        public async Task<IActionResult> GetApplicationDetails(string ApplicationNo = "")
        {
            try
            {

                HelpDeskServiceDTO data = await uow.IAdminMaster.GetApiTicketDetail(ApplicationNo.Trim());
                if (data != null)
                {
                    data.EncTicket = AESEncription.Base64Encode(data.TicketID);
                    data.Url = "https://helpdesk.agrauniv.online/Home/Tickets/?Ticket=" + data.EncTicket;
                    return Ok(data);
                }
               // return Ok();

            }
            catch (Exception ex)
            {

            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateServicesTicket(string ApplicationNo = "")
        {
            try
            {

                ServicesDTO smodel = await uow.IAdminMaster.GetServicesDetails(ApplicationNo);

                TblHelpDesk_AM model = new TblHelpDesk_AM();
                model.Enroll_Roll = smodel.EnrollmentNo;
                model.Roll = smodel.RollNo;
                model.Mobile = smodel.Mobile;
                model.Email = smodel.Email;
                model.Name = smodel.Name;
                model.CTypeID = 16;
                model.ApplicationNo = smodel.ApplicationNo;

                model.IsUser = CurrentUser == null ? true : false;
                model.UpdatedBy = CurrentUser == null ? null : (int)CurrentUser.UserId;
                model.IpAddress = await Geoloc();

                model.Subject = smodel.Remarks + "-" + smodel.CourseName+"-"+ model.ApplicationNo;
                model.Remarks = "<div>Your Application of Duplicate Marksheet has been Received Successfully and sent to concern department for processing. </br> Please Check status of Duplicate Marksheet Printing after 10 working days...</div>";
                FormResponse stat = await uow.IAdminMaster.SaveTicket(model);
                if (stat.ResponseCode == 1 && stat.ResponseMessageCode != "-1")
                {
                    HelpDeskServiceDTO s = new HelpDeskServiceDTO();
                    s.ApplicationNo = smodel.ApplicationNo;
                    s.TicketID = stat.ResponseMessageCode;
                    s.EncTicket = AESEncription.Base64Encode(stat.ResponseMessageCode);
                    s.Url = "https://helpdesk.agrauniv.online/Home/Tickets/?Ticket=" + s.EncTicket;
                    await uow.IAdminMaster.SaveSevicesUpdate(s);

                    return Ok(new { data = "success" });
                }


            }
            catch (Exception e)
            {
            }
            return Ok("fail");

        }
        public async Task<string> Geoloc()
        {
            try
            {
                string _ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                if (_ipAddress.Contains("::1"))
                    _ipAddress = "127.0.0.1";

                return _ipAddress;
            }
            catch (Exception e)
            {

            }
            return string.Empty;
        }
        // GET: api/<ApiController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ApiController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ApiController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

    }
}
