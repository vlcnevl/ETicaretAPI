﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.User
{
    public class LoginUser
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }
    }
}
