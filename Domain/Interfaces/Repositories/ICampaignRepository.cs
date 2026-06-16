using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces.Repositories
{
        public interface ICampaignRepository : IRepository<Campaign>
        {
                public Task<IEnumerable<Campaign>> GetActiveAsync();
        }
}
