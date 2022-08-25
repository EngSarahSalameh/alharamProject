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




namespace alharamApp.Controllers
{
    public class accountController : Controller
    {
        alharamDBEntities dbAccess = new alharamDBEntities();

        alharamDBFacilities dbAccessFacilitie = new alharamDBFacilities();



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
                    //its for my security purpose only 
                    userInfo.roleID = 1;

                    dbAccess.allUsers.Add(allUser userInfo);

                    //without save changes the previous insert query will not applied 
                    dbAccess.SaveChanges();

                    sendEmail(userInfo);
                }

                return RedirectToAction("loginForm");
            }
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
                ModelState.AddModelError("", "The email or password are incorrect");
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

                ModelState.AddModelError("", user.password);
                return View("forgetForm");
            }

            else
            {
                ModelState.AddModelError("", "Email does not exist");
                return View("forgetForm");
            }

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


        public ActionResult hotelManagement()
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

                return RedirectToAction("hotelManagement");
            }

            else
            {
                TempData["Message"] = "Error : The File Should Be Image Of Type png Or jpg Or jpeg";
                return RedirectToAction("hotelManagement");
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
                return RedirectToAction("hotelManagement");
            }

            else
            {
                TempData["Message"] = "Error : Not Exist Item";
                return RedirectToAction("hotelManagement");
            }
        }


        public ActionResult updateHotelName(int hotelID , string hotelName)
        {

            hotel updateHotelName = dbAccessFacilitie.hotels.First(a => a.hotelID == hotelID);
            updateHotelName.hotelName = hotelName;

            dbAccessFacilitie.SaveChanges();

            TempData["Message"] = "The Item Name Updated Successfully";
            return RedirectToAction("hotelManagement");
        }

        public ActionResult updateHotelLocation(int hotelID, string hotelLocation)
        {

            hotel updateHotelLocation = dbAccessFacilitie.hotels.First(a => a.hotelID == hotelID);
            updateHotelLocation.hotelLocation = hotelLocation;

            dbAccessFacilitie.SaveChanges();

            TempData["Message"] = "The Item Location Updated Successfully";
            return RedirectToAction("hotelManagement");
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

                return RedirectToAction("hotelManagement");
            }

            else
            {
                TempData["Message"] = "Error : The File Should Be Image Of Type png Or jpg Or jpeg";
                return RedirectToAction("hotelManagement");
            }

        }

     
    
        //Loggedin user Pages

        //public ActionResult loggedInuserInfo()//this code display all logged in user info
        //{
        //    string currentUser = User.Identity.Name;

        //    allUser user = dbAccess.allUsers.FirstOrDefault(x => x.email == currentUser);

        //    // string userFullName = user.firstName + " " + user.lastName;

        //    return View("", user);
        //}



    }//class end
}//name space end 
