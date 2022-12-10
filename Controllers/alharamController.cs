using alharamApp.Models;
using alharamApp.myData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using alharamApp;
using System.Net;
using System.Net.Mail;

namespace alharamApp.Controllers
{
    public class alharamController : Controller
    {
        alharamDBEntities dbAccess = new alharamDBEntities();

        alharamDBFacilities dbAccessFacilitie = new alharamDBFacilities();

        public ViewResult Index()
        {
            return View("Index");
        }

       
        public ViewResult about()
        {
            return View("about");
        }


        public ViewResult aboutVision()
        {
            return View("aboutVision");
        }

      
  
   
        public ActionResult contact()
        {

            return View("contact");
        }


        //Display all places

        public ActionResult showAllBarbershop()
        {
            List<facilitie> barbershops = new List<facilitie>();

            alharamDAO barbershopDAO = new alharamDAO();

            barbershops = barbershopDAO.fetchAllBarbershops();

            return View("showAllBarbershop", barbershops);

        }
   

        public ActionResult showAllHotel()
        {
            List<facilitie> hotels = new List<facilitie>();

            alharamDAO hotelDAO = new alharamDAO();

            hotels = hotelDAO.fetchAllHotels();

            return View("showAllHotel", hotels);

        }


        public ActionResult showAllRestaurant()
        {
            List<facilitie> restaurants = new List<facilitie>();

            alharamDAO restaurantDAO = new alharamDAO();

            restaurants = restaurantDAO.fetchAllRestaurants();

            return View("showAllRestaurant", restaurants);

        }

        //Display place depend on user search

           public ViewResult emptyRestaurant()
           {

            return View("emptyRestaurant");

           }

         public ViewResult emptyHotel()
          {

            return View("emptyHotel");

         }

        public ViewResult emptyBarbershop()
        {

            return View("emptyBarbershop");

        }

        [HttpPost]
        public ActionResult searchSomeBarbershop(string searchBarbershop)
        {
            List<facilitie> someBarbershops = new List<facilitie>();

            alharamDAO barbershopDAO = new alharamDAO();

            string search = searchBarbershop;

            someBarbershops = barbershopDAO.searchBarbershops(search);

            if (someBarbershops.Count == 0)
            {
                return View("emptyBarbershop");
            }

            else
            {
                return View("showAllBarbershop", someBarbershops);
            }

        }

        [HttpPost]
        public ActionResult searchSomeHotel(string searchHotel)
        {
            List<facilitie> someHotels = new List<facilitie>();

            alharamDAO hotelDAO = new alharamDAO();

            string search = searchHotel;
            someHotels = hotelDAO.searchHotels(search);

            if (someHotels.Count == 0)
            {
                return View("emptyHotel");
            }
            else
            {
                return View("showAllHotel", someHotels);
        }

    }


        [HttpPost]
        public ActionResult searchSomeRestaurant(string searchRestaurant)
        {
            List<facilitie> someRestaurants = new List<facilitie>();

            alharamDAO restaurantDAO = new alharamDAO();

            string search1 = searchRestaurant;

            someRestaurants = restaurantDAO.searchRestaurants(search1);

            if (someRestaurants.Count == 0)
            {
                return View("emptyRestaurant");
            }

            else
            {
                return View("showAllRestaurant", someRestaurants);
            }

        }

        [HttpPost]
        public ViewResult contactMessage(string name , string email, string subject, string message)
        {
            TempData["Message"] = "Dear " + name +" , thank you for contacting us, we always welcome your inquiries and comments";

            List<string> userComment = new List<string> { name, email, subject ,message };
           
            sendCommentEmail(userComment);

            return View("contactMessage");
        }

        public void sendCommentEmail(List<string> userComment)
        {
            MailMessage mail = new MailMessage();

            mail.To.Add("sarahsalameh59@gmail.com");

            string userEmail = userComment[1];

            mail.From = new MailAddress(userEmail);

            mail.Subject = userComment[2];

            mail.Body = "This Email Come To You From: " + userEmail + " , The Message is : <br/><br/>" + userComment[3];

            mail.IsBodyHtml = true;

            SmtpClient smtp = new SmtpClient();

            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Host = "smtp.gmail.com";
            smtp.Credentials = new System.Net.NetworkCredential("sarahsalameh59@gmail.com", "cvovqokoxtflfxfe");
            smtp.Send(mail);
        }
    }//class end 
    }//name space end