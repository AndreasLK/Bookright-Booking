using Microsoft.AspNetCore.Components;
using Facade.Bookings;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Client.Pages.Bookings
{
        public partial class BookingsSearch
        {
                [Inject] private BookingService BookingService { get; set; } = default!;
                [Inject] private NavigationManager NavigationManager { get; set; } = default!;

                private List<BookingSummaryDto>? _bookings;

                // Paging configurations
                private int _currentSkip = 0;
                private readonly int _takeAmount = 100;

                // State trackers
                private bool _isLoading = false;
                private bool _hasMoreData = true;

                // Sorting filters
                private BookingSortField _currentSortField = BookingSortField.CreatedAt;
                private SortDirection _currentSortDirection = SortDirection.Descending;

                protected override async Task OnInitializedAsync()
                {
                        _bookings = new List<BookingSummaryDto>();
                        await this.LoadNextBatchAsync();
                }

                private async Task LoadNextBatchAsync()
                {
                        // Unhappy Path First: Block extra execution requests if already busy or out of data
                        if (_isLoading || !_hasMoreData)
                        {
                                return;
                        }

                        _isLoading = true;

                        var nextBatch = await BookingService.GetBookingsAsync(
                            skip: _currentSkip,
                            take: _takeAmount,
                            sortField: _currentSortField,
                            sortDirection: _currentSortDirection
                        );

                        // Unhappy Path First: If the chunk returned nothing, flag end of records and exit early
                        if (nextBatch == null || !nextBatch.Any())
                        {
                                _hasMoreData = false;
                                _isLoading = false;
                                return;
                        }

                        // Happy Path execution
                        var nextBatchList = nextBatch.ToList();
                        _bookings ??= new List<BookingSummaryDto>();
                        _bookings.AddRange(nextBatchList);
                        _currentSkip += _takeAmount;

                        // If we fetched less than requested, we reached the end of the data store
                        if (nextBatchList.Count < _takeAmount)
                        {
                                _hasMoreData = false;
                        }

                        _isLoading = false;
                }

                private async Task ToggleSortAsync(BookingSortField field)
                {
                        // Unhappy Path First: Prevent sorting manipulation while a page batch is loading
                        if (_isLoading)
                        {
                                return;
                        }

                        // Determine direction toggles
                        if (_currentSortField == field)
                        {
                                _currentSortDirection = _currentSortDirection == SortDirection.Ascending
                                    ? SortDirection.Descending
                                    : SortDirection.Ascending;
                        }
                        else
                        {
                                _currentSortField = field;
                                _currentSortDirection = SortDirection.Descending;
                        }

                        // Reset pagination state variables to clear out history lists
                        _currentSkip = 0;
                        _hasMoreData = true;

                        if (_bookings is not null)
                        {
                                _bookings.Clear();
                        }

                        await this.LoadNextBatchAsync();
                }

                private string GetSortIcon(BookingSortField field)
                {
                        // Unhappy Path First: Return an empty indicator if this is not the active column field
                        if (_currentSortField != field)
                        {
                                return "↕";
                        }

                        return _currentSortDirection == SortDirection.Ascending ? "▲" : "▼";
                }

                private void HandleBookingSelected(BookingSummaryDto booking)
                {
                        // Unhappy Path First: Guard against malicious click events missing valid contextual items
                        if (booking == null || booking.Id == Guid.Empty)
                        {
                                return;
                        }

                        NavigationManager.NavigateTo($"/bookings/{booking.Id}");
                }
        }
}
