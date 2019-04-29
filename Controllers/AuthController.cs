using Microsoft.AspNetCore.Mvc;
using STOApi.Interfaces;
using Microsoft.AspNetCore.Cors;

namespace STOApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowSpecificOrigin")]
    public class AuthController : Controller
    {
        public IAuthRepository authRepository { set; get; }

        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpPost]
        public JsonResult Login(string username, string password)
        {
            return Json(authRepository.Login(username, password));
        }

        [HttpPost]
        public JsonResult AddUser(string username, string password, string repeatPassword, string role)
        {
            return Json(authRepository.AddUser(username, password, repeatPassword, role));
        }

        [HttpGet]
        public JsonResult GetUsers(){
            return Json(authRepository.GetUsers());
        }
    }
}