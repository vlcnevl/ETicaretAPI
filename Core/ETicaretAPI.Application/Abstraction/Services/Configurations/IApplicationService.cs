using ETicaretAPI.Application.DTOs.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Services.Configurations
{
    public interface IApplicationService
    {
        List<Menu> GetAuthhorizeDefinitionEndpoints(Type type); //authorize definition ile attribute ile işaretlenmiş tüm endpointleri getirir.
    }
}
 