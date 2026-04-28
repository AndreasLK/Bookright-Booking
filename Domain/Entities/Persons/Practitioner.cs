using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.Persons;
using Domain.Enums;

namespace Domain.Entities.People
{
        public class Practitioner : Person
        {
                public List<Guid> CertificateId = new List<Guid>();

                public string? Alias { get; private set; }


        }

}
