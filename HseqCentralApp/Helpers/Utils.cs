﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HseqCentralApp.Models;

namespace HseqCentralApp.Helpers
{
    public sealed class Utils
    {
        private Utils() { }

        private static ApplicationDbContext db = new ApplicationDbContext();

        public static ApplicationUser GetCurrentUser()
        {

            ApplicationUser currentUser = db.Users.Where(m => m.Email == HttpContext.Current.User.Identity.Name).First();
            return currentUser;

        }

        public static HseqUser GetCurrentApplicationUser()
        {

            ApplicationUser currentUser = GetCurrentUser();
            HseqUser hseqUser = db.HseqUsers.Where(a => a.UserID == currentUser.Id).First();
            return hseqUser;
        }

        public static HseqUser GetCurrentApplicationUser(ApplicationDbContext db)
        {

            ApplicationUser currentUser = GetCurrentUser();
            HseqUser hseqUser = db.HseqUsers.Where(a => a.UserID == currentUser.Id).First();
            return hseqUser;
        }


    }
}