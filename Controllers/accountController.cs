using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using alharamApp.Models;
using System.Linq;
using System.IO;

using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;




namespace alharamApp.Controllers
{
    public class accountController : Controller
    {
        alharamDBEntities dbAccess = new alharamDBEntities();

        alharamDBFacilities dbAccessFacilitie = new alharamDBFacilities();

        alharamDBFav dbAccessFav = new alharamDBFav();

        alharamDBAllPlaces dbAccessAllPlaces = new alharamDBAllPlaces();



        public ActionResult registerForm()
        {
            return View("registerForm");
        }


        public ActionResult loginForm()
        {
            return View("loginForm");
        }


        public ActionResult forgetForm()
        {
            return View("forgetForm");
        }



        //POST "When the user click submit button in previous forms the inforamtion will come here and those method will take care of database check and insert "
        [HttpPost]
        public ActionResult registerProcess(allUser userInfo)
        {
            //check if the email already used
            bool isEmailUsed = dbAccess.allUsers.Any(x => x.email == userInfo.email);

            if (isEmailUsed)
            {
                ModelState.AddModelError("", "The email already used");
                return View("registerForm");
            }

            else
            {
                if (userInfo.roleID == null)
                {
                    //its fro my security purpose only 
                    userInfo.roleID = 1;

                    dbAccess.allUsers.Add(userInfo);

                    //without save changes the previous insert query will not applied 
                    dbAccess.SaveChanges();

                    sendRegisterEmail(userInfo);
                }

                return RedirectToAction("loginForm");
               
            }
        }

        public void sendRegisterEmail(allUser userInfo)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(userInfo.email.ToString().Trim());

            mail.From = new MailAddress("sarahsalameh59@gmail.com");

            mail.Subject = "Welcome to Guide of the Alharam";

            mail.Body= "<p>Dear, " + userInfo.firstName + " " + userInfo.lastName + @"<br/>Welcome to Guide of the Alharam, we are glad you joined the site
             <br/>You can now log in through the login page that you were directed to and take advantage of the site's services</p>";

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();

            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("sarahsalameh59@gmail.com", "cvovqokoxtflfxfe");
            smtp.Send(mail);

        }
        
      

        [HttpPost]
        public ActionResult loginProcess(login existsOrNot)
        {
            bool isExists = dbAccess.allUsers.Any(x => x.email == existsOrNot.email && x.password == existsOrNot.password);

            //this will bring to us one record that match the condition
            allUser user = dbAccess.allUsers.FirstOrDefault(x => x.email == existsOrNot.email && x.password == existsOrNot.password);

            if (isExists)
            {
                //this create cookie that save the inforamtion of users then does not sign out attomatically 
                FormsAuthentication.SetAuthCookie(user.email, false);
                return RedirectToAction("Index", "alharam");
            }

            else
            {
                ModelState.AddModelError("", "Incorrect Password Or Email");
                return View("loginForm");
            }

        }


        [HttpPost]
        public ActionResult forgetProcess(login forgetPassword)
        {
            //check if the email Exist
            bool isEmailExist = dbAccess.allUsers.Any(x => x.email == forgetPassword.email);

            if (isEmailExist) //then if the email Exist send the password to user email
            {
               allUser user = dbAccess.allUsers.FirstOrDefault(x => x.email == forgetPassword.email);

                //ModelState.AddModelError("", user.password);
                ModelState.AddModelError("", "Check your email");

                sendForgetEmail(user);

                return View("forgetForm");
            }

            else
            {
                ModelState.AddModelError("", "Email does not exist");
                return View("forgetForm");
            }

        }

        public void sendForgetEmail(allUser user)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add(user.email.ToString().Trim());

            mail.From = new MailAddress("sarahsalameh59@gmail.com");

            mail.Subject = "Password Recovery";

            mail.Body = "<p>Your password is, <bold>" + user.password + "<bold></p>";

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();

            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("sarahsalameh59@gmail.com", "cvovqokoxtflfxfe");
            smtp.Send(mail);
        }

        //Logout

        public ActionResult logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "alharam");

        }


        //Admin Pages
      
        public ActionResult adminOperations()
        {
            return View("adminOperations");
        }

        [Authorize]
        public ActionResult OnlySecritOperation010()
        {
           
           List<hotel> hotels = dbAccessFacilitie.hotels.ToList();

            return View("hotelManagement" , hotels);
        }


        [HttpPost]
        public ActionResult addHotel(string hotelName , string hotelLocation , HttpPostedFileBase fileImage)
        {
            
            //Extract image name 
            string imgName = Path.GetFileName(fileImage.FileName);

            //Extract image extenstion 
            string FileExtension = imgName.Substring(imgName.LastIndexOf('.') + 1).ToLower();

            //accpect upload only if its image "check by extentions"
            if (FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "jpg") { 

                //set the path to image 
                string imgPath = "~/imgFacilities/hotelImg/" + imgName;

                //save the image file in folder 
                fileImage.SaveAs(Server.MapPath(imgPath));

                dbAccessFacilitie.hotels.Add(new hotel
                {
                    hotelName = hotelName,
                    hotelLocation = hotelLocation,
                    hotelImg = "imgFacilities/hotelImg/" + imgName
                });

                dbAccessFacilitie.SaveChanges();

                TempData["Message"] = "The Item Added Successfully";

                return RedirectToAction("OnlySecritOperation010");
            }

            else
            {
                TempData["Message"] = "Error : The File Should Be Image Of Type png Or jpg Or jpeg";
                return RedirectToAction("OnlySecritOperation010");
            }

        }



        public ActionResult deleteHotel(int hotelID)
        {
            int deteteByID = hotelID;

            hotel hotelDelete = dbAccessFacilitie.hotels.Find(deteteByID);

            if (hotelDelete != null)
            {
                dbAccessFacilitie.hotels.Remove(hotelDelete);
                dbAccessFacilitie.SaveChanges();

                TempData["Message"] = "The Item Deleted Successfully";
                return RedirectToAction("OnlySecritOperation010");
            }

            else
            {
                TempData["Message"] = "Error : Not Exist Item";
                return RedirectToAction("OnlySecritOperation010");
            }
        }


        public ActionResult updateHotelName(int hotelID , string hotelName)
        {

            hotel updateHotelName = dbAccessFacilitie.hotels.First(a => a.hotelID == hotelID);
            updateHotelName.hotelName = hotelName;

            dbAccessFacilitie.SaveChanges();

            TempData["Message"] = "The Item Name Updated Successfully";
            return RedirectToAction("OnlySecritOperation010");
        }

        public ActionResult updateHotelLocation(int hotelID, string hotelLocation)
        {

            hotel updateHotelLocation = dbAccessFacilitie.hotels.First(a => a.hotelID == hotelID);
            updateHotelLocation.hotelLocation = hotelLocation;

            dbAccessFacilitie.SaveChanges();

            TempData["Message"] = "The Item Location Updated Successfully";
            return RedirectToAction("OnlySecritOperation010");
        }


        [HttpPost]
        public ActionResult updateHotelImage(int hotelID, HttpPostedFileBase hotelImage)
        {

            //Extract image name 
            string imgName = Path.GetFileName(hotelImage.FileName);

            //Extract image extenstion 
            string FileExtension = imgName.Substring(imgName.LastIndexOf('.') + 1).ToLower();

            //accpect upload only if its image "check by extentions"
            if (FileExtension == "jpeg" || FileExtension == "png" || FileExtension == "jpg")
            {

                //set the path to image 
                string imgPath = "~/imgFacilities/hotelImg/" + imgName;

                //save the image file in folder 
                hotelImage.SaveAs(Server.MapPath(imgPath));

                hotel updateHotelImage = dbAccessFacilitie.hotels.First(a => a.hotelID == hotelID);
                updateHotelImage.hotelImg = "imgFacilities/hotelImg/" + imgName;

                dbAccessFacilitie.SaveChanges();

                TempData["Message"] = "The Item Image Updated Successfully";

                return RedirectToAction("OnlySecritOperation010");
            }

            else
            {
                TempData["Message"] = "Error : The File Should Be Image Of Type png Or jpg Or jpeg";
                return RedirectToAction("OnlySecritOperation010");
            }

        }


        /// <summary>
        /// add place to favorite and remove it from favorite all works
        /// </summary>
        /// <param name="ID"></param>

  
        [HttpPost]
        public ActionResult fav(int ID)
        {
            //insert to favorite list 

            string loggedUser = User.Identity.Name;

            //is the user already liked it ?
            bool isFavorated = dbAccessFav.userFavorites.Any(u => u.userIdentity == loggedUser && u.favoriteID == ID);

            if (isFavorated)
            {
                //Do nothing
            }

            else
            {

                dbAccessFav.userFavorites.Add(new userFavorite()
                {
                    userIdentity = loggedUser,
                    favoriteID = ID
                });

                dbAccessFav.SaveChanges();
            }

            return RedirectToAction("favProcess");
        }


        public ActionResult favProcess()
        {

            string loggedUser = User.Identity.Name;

            List<allPlace> favoriteList = new List<allPlace>();

            List<int?> favIDs = dbAccessFav.userFavorites.Where(u => u.userIdentity == loggedUser).Select(u => u.favoriteID).ToList();


            for (int i = 0; i < favIDs.Count; i++)
            {
                int convertedID = (int)favIDs[i];

                allPlace favPlace = dbAccessAllPlaces.allPlaces.FirstOrDefault(x => x.ID == convertedID);

                favoriteList.Add(favPlace);

            }

            return View("allFavoriteItems", favoriteList);
        }

        public ActionResult disLike(int ID)
        {

            string loggedUser = User.Identity.Name;

            userFavorite deleteFav = dbAccessFav.userFavorites.FirstOrDefault(u => u.userIdentity == loggedUser && u.favoriteID == ID);

            dbAccessFav.userFavorites.Remove(deleteFav);

            dbAccessFav.SaveChanges();

            return RedirectToAction("favProcess");
        }



        //Loggedin user account settings

        public ViewResult loggedInuserInfo()
        {
            return View("userAccountPage");
        }

        [HttpPost]
        public  ActionResult updateAccount(string fName , string lName , string email , string pass)
        {
                string loggedIn = User.Identity.Name;
                allUser userInfo = dbAccess.allUsers.First(a => a.email == loggedIn);

            if (email == loggedIn)
            {

                if (pass.Length < 7)
                {
                    TempData["PassLength"] = "The Password Must Be At Least 7 Characters Long";
                }
                else
                {
                    userInfo.firstName = fName;

                    userInfo.lastName = lName;

                    userInfo.password = pass;

                    TempData["Message"] = "Your Account Inforamtions Updated Successfully";
                }

                dbAccess.SaveChanges();

                return RedirectToAction("loggedInuserInfo");
            }

            else
            {
                userInfo.email = email;

                dbAccess.SaveChanges();

                FormsAuthentication.SignOut();

                TempData["Message"] = "Login With Your New Email";

                return RedirectToAction("loginForm");

            }
                
            

        }



    }//class end
}//name space end 