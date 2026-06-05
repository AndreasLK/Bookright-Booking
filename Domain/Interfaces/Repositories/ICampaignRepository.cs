using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repositories
{
        public interface ICampaignRepository : IRepository<Campaign>
        {
                Task<IEnumerable<Campaign>> GetActiveAsync();
        }
}
