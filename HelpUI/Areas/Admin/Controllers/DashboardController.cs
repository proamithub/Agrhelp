using HelpData.Entities;
using HelpModel;
using HelpModel.DTO;
using HelpModel.DTO.HelpDesk;
using HelpModel.DTO.Services;
using HelpModel.Enum;
using HelpRepo;
using HelpRepo.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace HelpUI.Areas.Admin.Controllers
{
    //[Area("Admin")]
    public class DashboardController : BaseController
    {
        int pageSize;
        private IWebHostEnvironment environment;
        public string ImgCloudPath = "";
        private IMailClient _mailClient;
        public DashboardController(IHttpContextAccessor _httpContextAccessor, IConfiguration _config,
             IWebHostEnvironment _environment, IUnitOfWork uow, IMailClient mailClient) : base(_httpContextAccessor, _config, uow)
        {
            environment = _environment;
            string _pageSize = "15";//Convert.ToString(WebConfigSetting.PageSize);
            int PS;
            bool result = Int32.TryParse(_pageSize, out PS);
            pageSize = (result == true) ? PS : 15;
            _mailClient = mailClient;
        }
        [Route("~/admin/dashboard")]
        public async Task<IActionResult> Index()
        {
            DashboardCount model = await UOF.IAdminMaster.GetHelpDeskCount((int)CurrentUser.UserId);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CountIndex()
        {
            DashboardCount model = await UOF.IAdminMaster.GetHelpDeskCount((int)CurrentUser.UserId);
            return PartialView("_CounterPage", model);
        }
        public async Task<IActionResult> NewTickectList(int ID=0)
        {
            List<TblHelpDesk_AM> s = await UOF.IAdminMaster.GetHelpDeskList(ENUM_HELP_STATUS.Pending.ToString(), (int)CurrentUser.UserId, ID);
            return View(s);
        }
        public async Task<IActionResult> OpenTickectList(int ID = 0)
        {
            List<TblHelpDesk_AM> s = await UOF.IAdminMaster.GetHelpDeskList(ENUM_HELP_STATUS.Open.ToString(), (int)CurrentUser.UserId, ID);
            return View(s);
        }
        public async Task<IActionResult> HoldTickectList(int ID = 0)
        {
            List<TblHelpDesk_AM> s = await UOF.IAdminMaster.GetHelpDeskList(ENUM_HELP_STATUS.OnHold.ToString(), (int)CurrentUser.UserId, ID);
            return View(s);
        }
        public async Task<IActionResult> ClosedTickectList(int ID = 0)
        {
            List<TblHelpDesk_AM> s = await UOF.IAdminMaster.GetHelpDeskList(ENUM_HELP_STATUS.Closed.ToString(), (int)CurrentUser.UserId, ID);
            return View(s);
        }
        public async Task<IActionResult> SpamTickectList(int ID = 0)
        {
            List<TblHelpDesk_AM> s = await UOF.IAdminMaster.GetHelpDeskList(ENUM_HELP_STATUS.Spam.ToString(), (int)CurrentUser.UserId, ID);
            return View(s);
        }
        
        [HttpGet]
        public async Task<IActionResult> Tickets(string Ticket,int t=0)
        {

            if (t == 1)
            {
                Ticket = AESEncription.Base64Decode(Ticket);
            }
            TblHelpDeskDTO_AM s = await UOF.IAdminMaster.GetTickets(Ticket);
            if (s != null)
            {
                // List<tb> s= await UOF.IAdminMaster.GetTickets(Ticket);
            }
            return View(s);
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
        [HttpPost]
        public async Task<IActionResult> Tickets(string Ticket, string Message, string Status)
        {
            TblHelpDeskHistories_AM model = new TblHelpDeskHistories_AM();
            model.TicketID = Ticket;
            model.Status = Status;
            model.Remarks = Message;
            model.Updatedby = (int)CurrentUser.UserId;
            model.IpAddress = await Geoloc();
            if (Request.Form.Files.Count > 0)
            {
                var errors = await ValidateFilesAsync(Request.Form.Files.ToList());

                if (errors.Count > 0)
                {
                    ViewBag.ErrorMessage = string.Join("<br>", errors);
                    TblHelpDeskDTO_AM s = await UOF.IAdminMaster.GetTickets(Ticket);
                    return View(s);
                }
                List<TblHelpFile_AM> docfile = new List<TblHelpFile_AM>();
                int j = 1;
                foreach (IFormFile file in Request.Form.Files)
                {
                    TblHelpFile_AM d = new TblHelpFile_AM();
                    var FullPath = "";

                    if ((file != null) && (file.Length > 0) && !string.IsNullOrEmpty(file.FileName))
                    {

                        var filename = ContentDispositionHeaderValue
                                        .Parse(file.ContentDisposition)
                                        .FileName
                                        .Trim('"');
                        var name = ContentDispositionHeaderValue
                                        .Parse(file.ContentDisposition)
                                        .Name
                                        .Trim('"');
                        d.Name = filename;
                        var ext = filename.Substring(filename.LastIndexOf('.'));
                        string time = DateTime.Now.ToString("yyyyMMddHHmmss");
                        string myfile = time+j.ToString() + ext;
                        var path = "D:/DOCUMENTS/HELPDESK_FILE/";
                        FullPath = path + myfile;
                        var uploads = Path.Combine(path);

                        var filePath = Path.Combine(uploads, myfile);

                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                        }

                        if (System.IO.File.Exists(filePath) == true)
                        {
                            System.IO.File.Delete(filePath);
                        }

                        using (FileStream fs = System.IO.File.Create(filePath))
                        {
                            file.CopyTo(fs);
                        }
                        d.FilePath = FullPath.Replace("D:", "");

                        //  size += file.Length;
                        j++;
                    }

                    docfile.Add(d);
                }
                model.FileList = docfile;
            }

            FormResponse adminData = await UOF.IAdminMaster.SaveRemarkTicket(model);
            //if (adminData.ResponseCode == 1 && adminData.ResponseMessageCode != "-1")
            //   {
            //       string t = AESEncription.Base64Encode(adminData.ResponseMessageCode);
            //       return RedirectToAction("Success", new { t=t });
            //       }
            return RedirectToAction("Tickets", new { Ticket = Ticket });
        }
        private async Task<List<string>> ValidateFilesAsync(List<IFormFile> files)
        {
            List<string> errors = new();

            string[] allowedExtensions =
            {
        ".jpg", ".jpeg", ".png", ".pdf"
    };

            long maxSize = 1 * 1024 * 1024; // 1 MB

            foreach (var file in files)
            {
                if (file == null || file.Length == 0)
                {
                    errors.Add($"{file?.FileName ?? "Unknown file"}: File is empty.");
                }
                else
                {
                    string extension = Path.GetExtension(file.FileName)
                                           .ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        errors.Add($"{file.FileName}: Invalid file type. Allowed Only .jpg,.jpeg,.png,.pdf");
                    }
                    else if (file.Length > maxSize)
                    {
                        double sizeMB = file.Length / (1024.0 * 1024.0);

                        errors.Add(
                            $"{file.FileName}: File size is {sizeMB:F2} MB. Maximum allowed is 1 MB."
                        );
                    }
                }

                // Stop after 2 errors
                //if (errors.Count >= 2)
                //    break;
            }

            await Task.CompletedTask;

            return errors;
        }

        [HttpPost]
        public async Task<IActionResult> FlagStatus([FromBody] TblHelpDesk_AM data)
        {
            try
            {

                FormResponse stat = await UOF.IAdminMaster.FlagStatus(data);
                if (stat.ResponseCode == 1 && stat.ResponseMessageCode != "-1")
                {
                    return Json(new { data = "success" });
                }
               

            }
            catch (Exception e)
            {
            }
            return Json("fail");

        }
        [HttpPost]
        public async Task<IActionResult> TransferTicket([FromBody] TblHelpDesk_AM data)
        {
            try
            {

                FormResponse stat = await UOF.IAdminMaster.TransferTicket(data,(int)CurrentUser.UserId);
                if (stat.ResponseCode == 1 && stat.ResponseMessageCode != "-1")
                {
                    return Json(new { data = "success" });
                }


            }
            catch (Exception e)
            {
            }
            return Json("fail");

        }
        
        public async Task<IActionResult> AdminMasterList()
        {
            List<AdminMasterDTO> model = await UOF.IAdminMaster.AdminMasterList();
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> AdminEdit(int AdminId) {
            try
            {
                AdminMasterDTO model=await UOF.IAdminMaster.AdminEdit(AdminId);
                model.Password= AESEncription.Base64Decode(model.Password);
                List<SelectListItem> Select_List = new List<SelectListItem>();
                foreach (var r in model.RolesDataList)
                {
                    SelectListItem obj = new SelectListItem()
                    {
                        Value = r.RoleId.ToString(),
                        Text = r.RoleName,
                        Selected = model.RolesSelectedList.Where(me => me.RoleId == r.RoleId).Count() > 0 ? true : false
                    };

                    Select_List.Add(obj);
                }
                model.RoleMaster = Select_List;

                List<SelectListItem> Select_Listct = new List<SelectListItem>();
                foreach (var r in model.CtypeList)
                {
                    SelectListItem obj = new SelectListItem()
                    {
                        Value = r.CTypeID.ToString(),
                        Text = r.TypeName,
                        Selected = model.CtySelectedList.Where(me => me.CTypeID == r.CTypeID).Count() > 0 ? true : false
                    };

                    Select_Listct.Add(obj);
                }
                model.CtMaster = Select_Listct;

                return View(model);
            }
            catch (Exception e)
            {
            }
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AdminEdit(AdminMasterDTO data)
        {
            try
            {
                data.Password = AESEncription.Base64Encode(data.Password);

                FormResponse model = await UOF.IAdminMaster.Update_AdminMasters(data);
                return RedirectToAction("AdminEdit", new { AdminId=data.AdminId });
            }
            catch (Exception e)
            {
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Deletechat([FromBody] TblHelpDeskHistories_AM model)
        {
            try
            {

                FormResponse stat = await UOF.IAdminMaster.Deletechat(model);
                if (stat.ResponseCode == 1 && stat.ResponseMessageCode != "-1")
                {
                    return Json(new { data = "success" });
                }


            }
            catch (Exception e)
            {
            }
            return Json("fail");

        }
        public async Task<IActionResult> ServiceList( string fromDate = "" ,string date = "" ,string SearchText="")
        {
            ViewBag.Date = date;
            ViewBag.SearchText = SearchText;
            ViewBag.fromDate = fromDate;
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!string.IsNullOrEmpty(fromDate))
            {
                //string[] date = date.Split('-');
                //startDate = Convert.ToDateTime(fromDate);
                startDate = Convert.ToDateTime(fromDate, CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
                //endDate = Convert.ToDateTime(date[1].Trim()).AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            else
            {
                startDate = DateTime.Today;
                ViewBag.fromDate = startDate.Value.ToString("dd/MMM/yyyy");
            }
            if (!string.IsNullOrEmpty(date))
            {
                //string[] date = date.Split('-');
                //endDate = Convert.ToDateTime(date);
                endDate = Convert.ToDateTime(date, CultureInfo.GetCultureInfo("hi-IN").DateTimeFormat);
            }
            else
            {
                endDate = DateTime.Today;
                ViewBag.date = endDate.Value.ToString("dd/MMM/yyyy");
            }
          
            List<ServicesDTO> model = await UOF.IAdminMaster.ServiceList(startDate, endDate, SearchText);

           
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateServicesTicket([FromForm] string ApplicationNo="")
        {
            try
            {

                ServicesDTO smodel = await UOF.IAdminMaster.GetServicesDetails(ApplicationNo);

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

                model.Subject = smodel.Remarks+"-"+smodel.CourseName;
                model.Remarks = "<div>Your Application of Duplicate Marksheet has been Received Successfully and sent to concern department for processing. </br> Please Check status of Duplicate Marksheet Printing after 10 working days.</div>";
                FormResponse stat =await UOF.IAdminMaster.SaveTicket(model);
                if (stat.ResponseCode == 1 && stat.ResponseMessageCode != "-1")
                {
                    HelpDeskServiceDTO s =new HelpDeskServiceDTO();
                    s.ApplicationNo=smodel.ApplicationNo;
                    s.TicketID=stat.ResponseMessageCode;
                    s.EncTicket = AESEncription.Base64Encode(stat.ResponseMessageCode);
                    s.Url = "https://helpdesk.agrauniv.online/Home/Tickets/?Ticket=" + s.EncTicket;
                    await UOF.IAdminMaster.SaveSevicesUpdate(s);

                    return Json(new { data = "success" });
                }


            }
            catch (Exception e)
            {
            }
            return Json("fail");

        }
        
    }
}
