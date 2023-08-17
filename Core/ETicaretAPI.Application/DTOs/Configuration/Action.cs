using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.Configuration
{
    public class Action
    {
        public string ActionType { get; set; }
        public string HttpType { get; set; } //get post put delete
        public string Definition { get; set; }
        public string Code { get; set; }
    }
}
