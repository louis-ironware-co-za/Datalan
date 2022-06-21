using DevExpress.Xpo;

using DxAimArb.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace DxAimArb.api.Upload
{
    [Microsoft.AspNetCore.Mvc.Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _contextAccessor;
        public UploadController(IWebHostEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._contextAccessor = httpContextAccessor;
        }

        //file uploads === 3 ==============================================================

        //Selfie With Id
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadSelfieWithID")]
        public ActionResult UploadSelfieWithID(IFormFile file) 
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\SelfieWithId", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);
                    
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //IdDocument
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadIdDocument")]
        public ActionResult UploadIdDocument(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\IdDocument", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Cheque Account
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadChequeAccount")]
        public ActionResult UploadChequeAccount(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\ChequeAccount", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Inheritance
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadInheritance")]
        public ActionResult UploadInheritance(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\Inheritance", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //InsurancePayout
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadInsurancePayout")]
        public ActionResult UploadInsurancePayout(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\InsurancePayout", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Investments
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadInvestments")]
        public ActionResult UploadInvestments(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\Investments", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //LoanRepayment
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadLoanRepayment")]
        public ActionResult UploadLoanRepayment(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\LoanRepayment", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //NCRLoan
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadNCRLoan")]
        public ActionResult UploadNCRLoan(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\NCRLoan", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //PrivateCompanyDividend
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadPrivateCompanyDividend")]
        public ActionResult UploadPrivateCompanyDividend(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\PrivateCompanyDividend", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //ProofOfAddress
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadProofOfAddress")]
        public ActionResult UploadProofOfAddress(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\ProofOfAddress", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //RetrenchmentAnnuity
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadRetrenchmentAnnuity")]
        public ActionResult UploadRetrenchmentAnnuity(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\RetrenchmentAnnuity", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //SaleOfAsset
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadSaleOfAsset")]
        public ActionResult UploadSaleOfAsset(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\SaleOfAsset", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Savings
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadSavings")]
        public ActionResult UploadSavings(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\Savings", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //SoleProprietor
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadSoleProprietor")]
        public ActionResult UploadSoleProprietor(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\SoleProprietor", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //SpousalDonations
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadSpousalDonations")]
        public ActionResult UploadSpousalDonations(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\SpousalDonations", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //TrustDistribution
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadTrustDistribution")]
        public ActionResult UploadTrustDistribution(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\TrustDistribution", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 1
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile1")]
        public ActionResult UploadFile1(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile1", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 2
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile2")]
        public ActionResult UploadFile2(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile2", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 3
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile3")]
        public ActionResult UploadFile3(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile3", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 4
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile4")]
        public ActionResult UploadFile4(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile4", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 5
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile5")]
        public ActionResult UploadFile5(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile5", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 6
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile6")]
        public ActionResult UploadFile6(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile6", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }

        //Bidvest File 7
        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile7")]
        public ActionResult UploadFile7(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads\\BidvestFile7", userID);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName)))
                {
                    //Check the file here.
                    file.CopyTo(fileStream);

                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
            return new EmptyResult();
        }
    }
}
