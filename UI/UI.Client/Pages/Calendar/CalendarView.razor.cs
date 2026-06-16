using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Facade.Clinics;
using Facade.Rooms;
using Facade.Common.Dtos;
using Facade.Customers;
using Facade.Calendar;

namespace UI.Client.Pages.Calendar
{
        /// <summary>
        /// Code-behind for the Main Calendar View component.
        /// </summary>
        public partial class CalendarView : ComponentBase, IAsyncDisposable
        {
                private const int CALENDAR_PAST_DAYS_VIEW = -14;
                private const int CALENDAR_FUTURE_DAYS_VIEW = 30;
                private const string JS_IMPORT_IDENTIFIER = "import";
                private const string JS_CALENDAR_MODULE_PATH = "./Pages/Calendar/CalendarView.razor.js";
                private const string JS_INITIALIZE_FUNCTION = "initializeCalendar";
                private const string JS_UPDATE_FUNCTION = "updateCalendarEvents";
                private const string CALENDAR_CONTAINER_ID = "calendar-container";

                /// <summary>
                /// Gets or sets the injected calendar facade.
                /// </summary>
                [Inject]
                private ICalendarFacade CalendarFacade { get; set; } = default!;

                /// <summary>
                /// Gets or sets the injected JS runtime.
                /// </summary>
                [Inject]
                private IJSRuntime JS { get; set; } = default!;

                protected List<ClinicDto> AvailableClinics { get; set; } = new();
                protected List<RoomDto> AvailableRooms { get; set; } = new();
                protected List<PractitionerLookupDto> AvailablePractitioners { get; set; } = new();
                protected List<CustomerSummaryDto> AvailableCustomers { get; set; } = new();
                protected List<TreatmentLookupDto> AvailableTreatments { get; set; } = new();

                protected List<ClinicDto> SelectedClinics { get; set; } = new();
                protected List<RoomDto> SelectedRooms { get; set; } = new();
                protected List<PractitionerLookupDto> SelectedPractitioners { get; set; } = new();
                protected List<CustomerSummaryDto> SelectedCustomers { get; set; } = new();

                protected List<CalendarEventViewModel> RenderedCalendarEvents { get; set; } = new();

                protected bool IsSmartModalVisible { get; set; }
                protected DateTime? DraggedStartTime { get; set; }
                protected DateTime? DraggedEndTime { get; set; }

                private DateTime _currentViewStart;
                private DateTime _currentViewEnd;

                private IJSObjectReference? _module;
                private DotNetObjectReference<CalendarView>? _dotNetRef;
                private bool _isCalendarInitialized = false;

                /// <summary>
                /// Initializes the calendar view component.
                /// </summary>
                protected override async Task OnInitializedAsync()
                {
                        this._currentViewStart = DateTime.Today.AddDays(value: CALENDAR_PAST_DAYS_VIEW);
                        this._currentViewEnd = DateTime.Today.AddDays(value: CALENDAR_FUTURE_DAYS_VIEW);

                        CalendarFilterLookupsDto lookups = await this.CalendarFacade.GetFilterLookupsAsync();

                        this.AvailableClinics = lookups.Clinics.ToList();
                        this.AvailableRooms = lookups.Rooms.ToList();
                        this.AvailableCustomers = lookups.Customers.ToList();
                        this.AvailablePractitioners = lookups.Practitioners.ToList();
                        this.AvailableTreatments = lookups.Treatments.ToList();

                        await this.RefreshCalendarDataAsync();
                }

                /// <summary>
                /// Handles after-render operations for JS interop setup.
                /// </summary>
                /// <param name="firstRender">Indicates whether this is the first render.</param>
                protected override async Task OnAfterRenderAsync(bool firstRender)
                {
                        if (!firstRender)
                        {
                                return;
                        }

                        this._dotNetRef = DotNetObjectReference.Create(value: this);

                        this._module = await this.JS.InvokeAsync<IJSObjectReference>(
                            identifier: JS_IMPORT_IDENTIFIER,
                            args: new object[] { JS_CALENDAR_MODULE_PATH }
                        );

                        await this._module.InvokeVoidAsync(
                            identifier: JS_INITIALIZE_FUNCTION,
                            args: new object[] { CALENDAR_CONTAINER_ID, this.RenderedCalendarEvents, this._dotNetRef }
                        );

                        this._isCalendarInitialized = true;
                }

                /// <summary>
                /// JS invokable method that handles drag-and-drop time slot selection.
                /// </summary>
                /// <param name="start">The string representation of the start date.</param>
                /// <param name="end">The string representation of the end date.</param>
                [JSInvokable]
                public void OnTimeSlotSelected(string start, string end)
                {
                        bool isStartValid = DateTime.TryParse(s: start, result: out DateTime startTime);
                        bool isEndValid = DateTime.TryParse(s: end, result: out DateTime endTime);

                        if (!isStartValid || !isEndValid)
                        {
                                return;
                        }

                        this.DraggedStartTime = startTime;
                        this.DraggedEndTime = endTime;
                        this.IsSmartModalVisible = true;
                        this.StateHasChanged();
                }

                /// <summary>
                /// Adds a clinic to the active filters.
                /// </summary>
                /// <param name="clinic">The clinic to add.</param>
                protected async Task AddClinicFilterAsync(ClinicDto clinic)
                {
                        if (clinic is null || this.SelectedClinics.Any(predicate: c => c.Id == clinic.Id))
                        {
                                return;
                        }

                        this.SelectedClinics.Add(item: clinic);
                        await this.RefreshCalendarDataAsync();
                        this.StateHasChanged();
                }

                /// <summary>
                /// Removes a clinic from the active filters.
                /// </summary>
                /// <param name="clinicToRemove">The clinic to remove.</param>
                protected async Task RemoveClinicFilterAsync(ClinicDto clinicToRemove)
                {
                        if (clinicToRemove is null)
                        {
                                return;
                        }

                        int removedCount = this.SelectedClinics.RemoveAll(match: c => c.Id == clinicToRemove.Id);

                        if (removedCount > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                /// <summary>
                /// Adds a room to the active filters.
                /// </summary>
                /// <param name="room">The room to add.</param>
                protected async Task AddRoomFilterAsync(RoomDto room)
                {
                        if (room is null || this.SelectedRooms.Any(predicate: r => r.Id == room.Id))
                        {
                                return;
                        }

                        this.SelectedRooms.Add(item: room);
                        await this.RefreshCalendarDataAsync();
                        this.StateHasChanged();
                }

                /// <summary>
                /// Removes a room from the active filters.
                /// </summary>
                /// <param name="roomToRemove">The room to remove.</param>
                protected async Task RemoveRoomFilterAsync(RoomDto roomToRemove)
                {
                        if (roomToRemove is null)
                        {
                                return;
                        }

                        int removedCount = this.SelectedRooms.RemoveAll(match: r => r.Id == roomToRemove.Id);

                        if (removedCount > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                /// <summary>
                /// Adds a practitioner to the active filters.
                /// </summary>
                /// <param name="practitioner">The practitioner to add.</param>
                protected async Task AddPractitionerFilterAsync(PractitionerLookupDto practitioner)
                {
                        if (practitioner is null || this.SelectedPractitioners.Any(predicate: p => p.Id == practitioner.Id))
                        {
                                return;
                        }

                        this.SelectedPractitioners.Add(item: practitioner);
                        await this.RefreshCalendarDataAsync();
                        this.StateHasChanged();
                }

                /// <summary>
                /// Removes a practitioner from the active filters.
                /// </summary>
                /// <param name="practitionerToRemove">The practitioner to remove.</param>
                protected async Task RemovePractitionerFilterAsync(PractitionerLookupDto practitionerToRemove)
                {
                        if (practitionerToRemove is null)
                        {
                                return;
                        }

                        int removedCount = this.SelectedPractitioners.RemoveAll(match: p => p.Id == practitionerToRemove.Id);

                        if (removedCount > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                /// <summary>
                /// Adds a customer to the active filters.
                /// </summary>
                /// <param name="customer">The customer to add.</param>
                protected async Task AddCustomerFilterAsync(CustomerSummaryDto customer)
                {
                        if (customer is null || this.SelectedCustomers.Any(predicate: c => c.Id == customer.Id))
                        {
                                return;
                        }

                        this.SelectedCustomers.Add(item: customer);
                        await this.RefreshCalendarDataAsync();
                        this.StateHasChanged();
                }

                /// <summary>
                /// Removes a customer from the active filters.
                /// </summary>
                /// <param name="customerToRemove">The customer to remove.</param>
                protected async Task RemoveCustomerFilterAsync(CustomerSummaryDto customerToRemove)
                {
                        if (customerToRemove is null)
                        {
                                return;
                        }

                        int removedCount = this.SelectedCustomers.RemoveAll(match: c => c.Id == customerToRemove.Id);

                        if (removedCount > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                /// <summary>
                /// Refreshes the calendar data from the facade based on selected filters.
                /// </summary>
                private async Task RefreshCalendarDataAsync()
                {
                        List<Guid> clinicIds = this.SelectedClinics.Select(selector: c => c.Id).ToList();
                        List<Guid> roomIds = this.SelectedRooms.Select(selector: r => r.Id).ToList();
                        List<Guid> practitionerIds = this.SelectedPractitioners.Select(selector: p => p.Id).ToList();
                        List<Guid> customerIds = this.SelectedCustomers.Select(selector: c => c.Id).ToList();

                        IEnumerable<CalendarEventViewModel> updatedEvents = await this.CalendarFacade.RefreshCalendarBookingsAsync(
                            viewStartDate: this._currentViewStart,
                            viewEndDate: this._currentViewEnd,
                            clinicIds: clinicIds,
                            roomIds: roomIds,
                            practitionerIds: practitionerIds,
                            customerIds: customerIds
                        );

                        this.RenderedCalendarEvents = updatedEvents.ToList();

                        if (this._isCalendarInitialized && this._module is not null)
                        {
                                await this._module.InvokeVoidAsync(
                                    identifier: JS_UPDATE_FUNCTION,
                                    args: new object[] { CALENDAR_CONTAINER_ID, this.RenderedCalendarEvents }
                                );
                        }

                        this.StateHasChanged();
                }

                /// <summary>
                /// Closes the smart booking modal and resets dragged time state.
                /// </summary>
                protected void CloseSmartModal()
                {
                        this.IsSmartModalVisible = false;
                        this.DraggedStartTime = null;
                        this.DraggedEndTime = null;
                }

                /// <summary>
                /// Handles successful booking save and refreshes calendar view.
                /// </summary>
                protected async Task HandleBookingSavedAsync()
                {
                        this.CloseSmartModal();
                        await this.RefreshCalendarDataAsync();
                }

                /// <summary>
                /// Disposes of async resources like JS modules.
                /// </summary>
                public async ValueTask DisposeAsync()
                {
                        if (this._module is not null)
                        {
                                try
                                {
                                        await this._module.DisposeAsync();
                                }
                                catch (JSDisconnectedException)
                                {
                                        // Ignore standard Blazor disconnects
                                }
                        }

                        this._dotNetRef?.Dispose();
                }
        }
}
