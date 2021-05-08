using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiscMVCWebApp.Services
{
    public class OperationService : ISingletonService, IScopedService, ITransientService
    {
        Guid id;

        public OperationService()
        {
            id = Guid.NewGuid();
        }

        public Guid GetOperationID()
        {
            return id;
        }
    }
}
