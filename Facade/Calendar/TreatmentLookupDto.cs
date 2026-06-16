using System;
using System.Collections.Generic;
using System.Text;

namespace Facade.Calendar
{
        public record TreatmentLookupDto(
        Guid Id,
        string Name,
        TimeSpan Duration
    );
}
