
using HelpUI.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using HelpModel;
using HelpModel.DTO;
using HelpRepo;
using System.Collections.Generic;
using System.Drawing;

namespace HelpUI.ViewComponents
{
    public class ResultViewComponent : ViewComponent
    {
        IUnitOfWork uow; 
        protected readonly IHttpContextAccessor httpContextAccessor;
        protected ISession Session => httpContextAccessor.HttpContext.Session;
        public ResultViewComponent(IUnitOfWork _uow, IHttpContextAccessor _httpContextAccessor)
        {
            httpContextAccessor = _httpContextAccessor;
            uow = _uow;
        }

        public async Task<IViewComponentResult> InvokeAsync(string data)
        {
            try
            {

                var s = Session.GetObject<SearchDTO>("resultdata");
                var roll = AESEncription.Base64Decode(data);
                if (roll == s.RollNumber)
                {
                    StudentMasterDTO adminData = await uow.IAdminMaster.ResultStudentData(s);
                    if (roll == adminData.ROLL_NO)
                    {
                        adminData.MarksList = await uow.IAdminMaster.ResultMarksStudentData(s);
                        adminData.ResultList = await uow.IAdminMaster.ResultDataStudentData(s);
                        adminData.CSPList = await uow.IAdminMaster.CSVDataStudentData(adminData);
                        adminData.EncrptedRoll = data;

                        var data1 = s.CourseName + "$" + s.RollNumber + "$" + s.CourseType + "$" + s.ExamTypeName;
                        data1 = AESEncription.Base64Encode(data1);
                        var UriPayload = "https://resulttest.agrauniv.online/Home/resultpreview/" + data1;
                        QRCodeGenerator _qrCode = new QRCodeGenerator();
                        QRCodeData _qrCodeData = _qrCode.CreateQrCode(UriPayload, QRCodeGenerator.ECCLevel.Q);
                        QRCode qrCode = new QRCode(_qrCodeData);
                        Bitmap qrCodeImage = qrCode.GetGraphic(20);
                        adminData.qcore = BitmapToBytesCode(qrCodeImage);

                        if (adminData.TemplateName == "T1")
                        {
                            return View("/views/components/_Result_t1.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T2")
                        {
                            return View("/views/components/_Result_t2.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T3")
                        {
                            return View("/views/components/_Result_t3.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T4")
                        {
                            return View("/views/components/_Result_t4.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T5")
                        {
                            return View("/views/components/_Result_t5.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T6")
                        {
                            return View("/views/components/_Result_t6.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T7")
                        {
                            return View("/views/components/_Result_t7.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T8")
                        {
                            return View("/views/components/_Result_t8.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T9")
                        {
                            return View("/views/components/_Result_t9.cshtml", adminData);
                        }
                        if (adminData.TemplateName == "T10")
                        {
                            return View("/views/components/_Result_t10.cshtml", adminData);
                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }

            return View("/views/components/_Result_t1.cshtml");

        }
        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}