using Microsoft.AspNetCore.Components;
using Facade.Bookings;
using System;
using System.Threading.Tasks;

namespace UI.Client.Pages.BookingList
{
        public partial class BookingEdit : ComponentBase
        {
                [Inject] private BookingService BookingService { get; set; } = default!;
                [Inject] private NavigationManager NavigationManager { get; set; } = default!;

                [Parameter]
                public Guid Id { get; set; }

                protected BookingSummaryDto? _booking;
                protected bool _isLoading = true;

                protected override async Task OnInitializedAsync()
                {
                        this._booking = await this.BookingService.GetBookingByIdAsync(id: this.Id);
                        this._isLoading = false;
                }

                /// <summary>
                /// Triggered by the GenericEditForm when validation passes.
                /// </summary>
                /// <param name="model">The mutated BookingSummaryDto from the form.</param>
                protected async Task HandleValidSubmitAsync(BookingSummaryDto model)
                {
                        if (model != null)
                        {
                                await this.BookingService.RescheduleBookingAsync(
                                    bookingId: this.Id,
                                    newStartTime: model.StartTime,
                                    newEndTime: model.EndTime
                                );

                                if (model.AmountPaid.HasValue)
                                {
                                        await this.BookingService.RegisterPaymentAsync(
                                            bookingId: this.Id,
                                            amountPaid: model.AmountPaid.Value
                                        );
                                }

                                this.NavigationManager.NavigateTo(uri: "/bookings");
                        }
                }

                /// <summary>
                /// Triggered by the GenericEditForm cancel button.
                /// </summary>
                protected void GoBack()
                {
                        this.NavigationManager.NavigateTo(uri: "/bookings");
                }
        }
}
