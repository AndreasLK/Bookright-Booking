using Facade.Bookings;
using Facade.Common.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Client.Pages.BookingList
{
        public partial class BookingEdit : ComponentBase
        {
                [Inject]
                private BookingService BookingService { get; set; } = default!;

                [Inject]
                private NavigationManager NavigationManager { get; set; } = default!;

                [Parameter]
                public Guid Id { get; set; }

                protected BookingSummaryDto? _booking;
                protected IEnumerable<PractitionerLookupDto> _practitioners = Array.Empty<PractitionerLookupDto>();

                protected bool _isLoading = true;
                protected bool _isPastBooking = false;

                protected override async Task OnInitializedAsync()
                {
                        this._booking = await this.BookingService.GetBookingByIdAsync(id: this.Id);
                        this._practitioners = await this.BookingService.GetAvailablePractitionersAsync();

                        if (this._booking is not null)
                        {
                                this._isPastBooking = this._booking.StartTime < DateTime.Now;
                        }

                        this._isLoading = false;
                }

                protected async Task HandleValidSubmitAsync(BookingSummaryDto model)
                {
                        if (model is not null)
                        {
                                await this.BookingService.RescheduleBookingAsync(
                                    bookingId: this.Id,
                                    newStartTime: model.StartTime,
                                    newEndTime: model.EndTime
                                );

                                if (!this._isPastBooking)
                                {
                                        await this.BookingService.ReassignPractitionerAsync(
                                            bookingId: this.Id,
                                            newPractitionerId: model.PractitionerId
                                        );
                                }

                                if (model.AmountPaid.HasValue)
                                {
                                        // FIXED: Parameter name changed from amountInDkk to amountPaid
                                        await this.BookingService.RegisterPaymentAsync(
                                            bookingId: this.Id,
                                            amountPaid: model.AmountPaid.Value
                                        );
                                }

                                this.NavigationManager.NavigateTo(uri: "/bookings");
                        }
                }

                protected void GoBack()
                {
                        this.NavigationManager.NavigateTo(uri: "/bookings");
                }
        }
}
