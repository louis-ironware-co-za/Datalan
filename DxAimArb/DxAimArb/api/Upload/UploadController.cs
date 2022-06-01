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

        [HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("UploadFile")]
        public ActionResult UploadFile(IFormFile file)
        {
            try
            {
                string userID = this._contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", userID);

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
