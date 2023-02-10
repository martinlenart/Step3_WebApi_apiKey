﻿using System.ComponentModel.DataAnnotations;

namespace Step3_WebApi_apiKey.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Guid apiKey { get; set; }
    }


    //used for Login
    public class UserCredentials
    {
        [Required]
        public string UserName { get; set; }  //Name or email

        [Required]
        public string Password { get; set; }
    }
}
