using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum RequestStatus
    {
        Pending = 1,    // Folyamatban
        Completed = 2,  // Átadva
        Failed = 3     // Sikertelen
    }
}
