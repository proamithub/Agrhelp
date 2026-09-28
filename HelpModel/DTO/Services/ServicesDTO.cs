using HelpModel.CustomAttribute;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace HelpModel.DTO.Services
{
    public class ServicesDTO
    {
        public string ApplicationNo { get; set; }
        public string? TicketID { get; set; }
        
        public int ApplicationTypeId { get; set; }
        public string? ApplicationTypeName { get; set; }

        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime? Dob { get; set; }
        public int PassingYear { get; set; }
        public string EnrollmentNo { get; set; }
        public string RollNo { get; set; }
        public string OldApplicationNo { get; set; }
        public int? FeeAmount { get; set; }
        public string Session { get; set; }
        public bool IsPaid { get; set; }
        public string Remarks { get; set; }
        public string IsPrinted { get; set; }
        public string? AdminRemarks { get; set; }
        public string? AdminRemarksBy { get; set; }
        public string AdminRemarksDate { get; set; }
        public string UpdatedDate { get; set; }
        public string TransactionId{ get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public Nullable<bool> IsActive { get; set; }

    }


    public class HelpDeskServiceDTO
    {
        public string ApplicationNo { get; set; }
        public string? TicketID { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? EncTicket { get; set; }
        public string? Url { get;set; } 
    }

    }
