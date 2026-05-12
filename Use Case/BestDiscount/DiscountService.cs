using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System.Threading;

namespace Use_Case.BestDiscount
{
        public class DiscountService
        {

                private readonly IBookingRepository _bookingRepository;
                private readonly ITreatmentRepository _treatmentRepository;

                public DiscountService(IBookingRepository bookingRepository, ITreatmentRepository treatmentRepository)
                {
                        ArgumentNullException.ThrowIfNull(bookingRepository);
                        ArgumentNullException.ThrowIfNull(treatmentRepository);

                        this._bookingRepository = bookingRepository;
                        this._treatmentRepository = treatmentRepository;
                }

                public Money GetBestDiscount(DiscountContext context)
                {
                        ArgumentNullException.ThrowIfNull(context);

                        ManualResetEvent[] workItemsDone = new ManualResetEvent[context.ActiveCampaigns.Count];

                        for(int i = 0; i < context.ActiveCampaigns.Count ;i++)
                        {
                                bool containsKey = context.TimeUsedEligbleCampaigns.ContainsKey(key: context.ActiveCampaigns[i].Id);
                                if (containsKey) continue;


                                List<DateTime> campaignUsage = context.TimeUsedEligbleCampaigns[context.ActiveCampaigns[i].Id];

                                workItemsDone[i] = new ManualResetEvent(initialState: false);

                                
                        }
                }
        }
}
