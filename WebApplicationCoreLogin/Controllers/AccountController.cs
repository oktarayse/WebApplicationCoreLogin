using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Security.Claims;
using WebApplicationCoreLogin.Models;
using WebApplicationCoreLogin.Models.ViewModel;

namespace WebApplicationCoreLogin.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private DatabaseContext db;
        private IConfiguration _configuration;
        public AccountController(DatabaseContext dbcontext,IConfiguration configuration )
        {
            db=dbcontext;
            _configuration=configuration;
        }
        //dependency injection(iş görüşmelerinde sorulur.)

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)//Model geçerliyse
            {
                 string sifre= _configuration.GetValue<string>("Appsettings:sifre");
                sifre=model.password+sifre;
                string md5sifre=sifre.MD5();


                User user=db.Users.SingleOrDefault(x=>x.UserName.ToLower()==model.userName.ToLower()&&x.Password==md5sifre);

                if (user!=null)
                {
                    List<Claim>claims=new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Role,user.Role));
                    claims.Add(new Claim("Name",user.Name?? string.Empty));
                    claims.Add(new Claim("UserName",user.UserName));

                    ClaimsIdentity claimsIdentity=new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);//Tırnak içden Cookie'de yazabiliriz.

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,new ClaimsPrincipal(claimsIdentity));

                    
                    

                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("","Kullanıcı adı ya da şifre hatalı");
                }
                
            }
            return View(model);
        }

          [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
          [AllowAnonymous]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
              
                    if (db.Users.Any(x=>x.UserName==model.userName.ToLower()))
                    {
                        ModelState.AddModelError(nameof(model.userName),"Bu kullanıcıadı sistemde bulunuyor");
                        return View(model);
                    }
                
               string sifre= _configuration.GetValue<string>("Appsettings:sifre");
                sifre=model.password+sifre;
                string md5sifre=sifre.MD5();
                

                User user =new User() //yeni olan versiyonda new yaznında user olammdanda çalışıo
                {
                    UserName=model.userName,
                    Password=md5sifre
                };
                db.Users.Add(user);
                if(db.SaveChanges()==0)
                {
                    ModelState.AddModelError("","Kayıt eklenemedi.");
                }
                else
                {
                   return RedirectToAction("Login");
                }

            }
            return View(model);
        }
       [AllowAnonymous]
         public IActionResult Profil()
        {
            ProfilBilgiGoster();
            return View();
        }

        public IActionResult ProfilResmiKaydet(IFormFile resim)
        {
            if (ModelState.IsValid)
            {
                  Guid id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = db.Users.SingleOrDefault(x => x.Id == id);
                 
            string filename=$"f_{id}.jpg";
            Stream stream=new FileStream($"wwwroot/image/{filename}",FileMode.OpenOrCreate);

            resim.CopyTo(stream);
            stream.Close();
            //eğer kullanılmdayısa dosya kapanmadı gibi bir uyarı verio şart değil
            stream.Dispose();

                user.ProfilePictureFile = filename;
                db.SaveChanges();

            
            return RedirectToAction("Profil");
            }
          
             return View("Profil");
        }

        private void ProfilBilgiGoster()
        {
            Guid id = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = db.Users.SingleOrDefault(x => x.Id == id);

            ViewData["adsoyad"] = user.Name;
            ViewData["username"] = user.UserName;
            ViewData["password"] = user.Password;
            ViewData["image"] = user.ProfilePictureFile;
            ViewData["mesaj"] = TempData["mesaj"];
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult AdSoyadKaydet(string adsoyad)
        {
            if (ModelState.IsValid)
            {
               Guid id=new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user=db.Users.SingleOrDefault(x=>x.Id==id);

                user.Name=adsoyad;
                db.SaveChanges();

                TempData["mesaj"]="NameUpdate";
                return RedirectToAction("Profil");

            }
            ProfilBilgiGoster();
            return View("Profil");
        } 
        public IActionResult UserNameSave(string username)
        {
            if (ModelState.IsValid)
            {



                ProfilBilgiGoster();
               Guid id=new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user=db.Users.SingleOrDefault(x=>x.Id==id);

                   if (db.Users.Any(x=>x.UserName.ToLower()==user.UserName.ToLower()&&x.Id!=id))
                    {
                        ModelState.AddModelError(nameof(user.UserName),"Bu kullanıcıadı sistemde bulunuyor");
                        return View("Profil");
                    }
                user.UserName=username;
                db.SaveChanges();

                TempData["mesaj"]="UserNameUpdate";
                return RedirectToAction("Profil");

            }
            ProfilBilgiGoster();
            return View("Profil");
        }
        public IActionResult PasswordSave(string password) 
            {
            if (ModelState.IsValid)
            {
                    Guid id=new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user=db.Users.SingleOrDefault(x=>x.Id==id);

                    string sifre= _configuration.GetValue<string>("Appsettings:sifre");
                sifre=password+sifre;
                string md5sifre=sifre.MD5();

                user.Password=md5sifre;
                    db.SaveChanges() ;

             
                
            }
            ProfilBilgiGoster();
            return View("Profil");
            }
         public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }


    }
}
