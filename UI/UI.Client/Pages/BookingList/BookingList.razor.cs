using Microsoft.AspNetCore.Components;
using Facade.Bookings;
using System;
using System.Collections.Generic;

namespace UI.Client.Pages.BookingList
{
        public partial class BookingList
        {
                /// <summary>
                /// The collection of bookings to display. 
                /// Pass a filtered list for a single customer, or the full list for a global view.
                /// </summary>
                [Parameter, EditorRequired]
                public IEnumerable<BookingSummaryDto> Bookings { get; set; } = Array.Empty<BookingSummaryDto>();

                /// <summary>
                /// Callback triggered when a booking card is clicked or double-clicked.
                /// </summary>
                [Parameter]
                public EventCallback<BookingSummaryDto> OnBookingSelected { get; set; }
        }
}
