using Microsoft.AspNetCore.Components;
using Facade.Bookings;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Client.Pages.BookingList
{
        /// <summary>
        /// Page component for searching, sorting, and viewing a paginated list of bookings.
        /// </summary>
        public partial class BookingSearch : ComponentBase
        {
                [Inject] private BookingService BookingService { get; set; } = default!;
                [Inject] private NavigationManager NavigationManager { get; set; } = default!;

                protected List<BookingSummaryDto>? _bookings;

                protected bool _isLoading = false;
                protected bool _hasMoreData = true;

                protected BookingSortOption _currentSortOption = BookingSortOption.StartTime;
                protected SortDirection _currentSortDirection = SortDirection.Ascending;

                private int _currentSkip = 0;
                private readonly int _takeAmount = 100;

                /// <inheritdoc/>
                protected override async Task OnInitializedAsync()
                {
                        this._bookings = new List<BookingSummaryDto>();
                        await this.LoadNextBatchAsync();
                }

                /// <summary>
                /// Fetches the next paginated chunk of bookings from the backend service.
                /// </summary>
                protected async Task LoadNextBatchAsync()
                {
                        if (this._isLoading || !this._hasMoreData)
                        {
                                return;
                        }

                        this._isLoading = true;

                        IEnumerable<BookingSummaryDto> nextBatch = await this.BookingService.GetBookingsAsync(
                                sortOption: this._currentSortOption,
                                sortDirection: this._currentSortDirection,
                                skip: this._currentSkip,
                                take: this._takeAmount
                        );

                        if (nextBatch == null || !nextBatch.Any())
                        {
                                this._hasMoreData = false;
                                this._isLoading = false;
                                return;
                        }

                        List<BookingSummaryDto> nextBatchList = nextBatch.ToList();

                        this._bookings ??= new List<BookingSummaryDto>();
                        this._bookings.AddRange(collection: nextBatchList);

                        this._currentSkip += this._takeAmount;

                        if (nextBatchList.Count < this._takeAmount)
                        {
                                this._hasMoreData = false;
                        }

                        this._isLoading = false;
                }

                /// <summary>
                /// Invoked when the user selects a new sorting field from the dropdown.
                /// </summary>
                /// <param name="newOption">The newly selected sort option.</param>
                protected async Task OnSortOptionChangedAsync(BookingSortOption newOption)
                {
                        if (this._currentSortOption == newOption)
                        {
                                return;
                        }

                        this._currentSortOption = newOption;
                        await this.ResetAndReloadAsync();
                }

                /// <summary>
                /// Invoked when the user selects a new sorting direction from the dropdown.
                /// </summary>
                /// <param name="newDirection">The newly selected sort direction.</param>
                protected async Task OnSortDirectionChangedAsync(SortDirection newDirection)
                {
                        if (this._currentSortDirection == newDirection)
                        {
                                return;
                        }

                        this._currentSortDirection = newDirection;
                        await this.ResetAndReloadAsync();
                }

                /// <summary>
                /// Navigates to the details/edit page for the selected booking.
                /// </summary>
                /// <param name="booking">The booking DTO selected by the user.</param>
                protected void HandleBookingSelected(BookingSummaryDto booking)
                {
                        if (booking == null || booking.Id == Guid.Empty)
                        {
                                return;
                        }

                        this.NavigationManager.NavigateTo(uri: $"/bookings/{booking.Id}");
                }

                /// <summary>
                /// Clears the current list and fetches data from the beginning with the new sorting rules.
                /// </summary>
                private async Task ResetAndReloadAsync()
                {
                        this._currentSkip = 0;
                        this._hasMoreData = true;

                        this._bookings?.Clear();

                        await this.LoadNextBatchAsync();
                }
        }
}
