using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericPaymentService.Models
{
    public class PayeeDetails
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string TelephoneLandline { get; set; }
        public string TelephoneMobile { get; set; }

        public PayeeDetails()
        {
            Title = "";
            FirstName = "";
            Surname = "";
            AddressLine1 = "";
            AddressLine2 = "";
            AddressLine3 = "";
            Postcode = "";
            Town = "";
            County = "";
            Country = "";
            Email = "";
            TelephoneLandline = "";
            TelephoneMobile = "";
        }
    }
}
