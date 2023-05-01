using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Stroage
{
    public interface IStroageService : IStroage
    {
        public string StroageName { get; }
    }
}
