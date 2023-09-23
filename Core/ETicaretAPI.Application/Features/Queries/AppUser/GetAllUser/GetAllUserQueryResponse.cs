using ETicaretAPI.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.AppUser.GetAllUser
{
    public class GetAllUserQueryResponse
    {
       public object Users {  get; set; }
       public int TotalCount { get; set; }
    }
}
