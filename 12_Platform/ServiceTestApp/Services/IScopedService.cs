using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiscMVCWebApp.Services
{
    public interface IScopedService
    {
        Guid GetOperationID();
    }
}
