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
        public partial class CalendarView : ComponentBase, IAsyncDisposable
        {
                private const string JS_IMPORT_IDENTIFIER = "import";
                private const string JS_CALENDAR_MODULE_PATH = "./Pages/Calendar/CalendarView.razor.js";
                private const string JS_INITIALIZE_FUNCTION = "initializeCalendar";
                private const string JS_UPDATE_FUNCTION = "updateCalendarEvents";
                private const string JS_SET_DURATION_FUNCTION = "setTreatmentPreviewDuration";
                private const string TIMESPAN_FORMAT = @"hh\:mm\:ss";
                private const string CALENDAR_CONTAINER_ID = "calendar-container";

                [Inject]
                private ICalendarFacade CalendarFacade { get; set; } = default!;

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

                public Guid? SelectedTreatmentIdForBooking { get; private set; }
                public TimeSpan? SelectedTreatmentDuration { get; private set; }
                public string WarningMessage { get; private set; } = string.Empty;

                protected bool IsSmartModalVisible { get; set; }
                protected DateTime? DraggedStartTime { get; set; }
                protected DateTime? DraggedEndTime { get; set; }

                private DateTime _currentViewStart = DateTime.Today.AddDays(value: -14);
                private DateTime _currentViewEnd = DateTime.Today.AddDays(value: 30);

                private IJSObjectReference? _module;
                private DotNetObjectReference<CalendarView>? _dotNetRef;
                private bool _isCalendarInitialized = false;

                protected override async Task OnInitializedAsync()
                {
                        CalendarFilterLookupsDto lookups = await this.CalendarFacade.GetFilterLookupsAsync();

                        this.AvailableClinics = lookups.Clinics.ToList();
                        this.AvailableRooms = lookups.Rooms.ToList();
                        this.AvailableCustomers = lookups.Customers.ToList();
                        this.AvailablePractitioners = lookups.Practitioners.ToList();
                        this.AvailableTreatments = lookups.Treatments.ToList();

                        await this.RefreshCalendarDataAsync();
                }

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

                public async Task OnTreatmentSelected(ChangeEventArgs e)
                {
                        if (e is null || e.Value is null)
                        {
                                return;
                        }

                        if (Guid.TryParse(input: e.Value.ToString(), result: out Guid id))
                        {
                                this.SelectedTreatmentIdForBooking = id;
                                TreatmentLookupDto? treatment = this.AvailableTreatments.FirstOrDefault(predicate: t => t.Id == id);

                                if (treatment is not null)
                                {
                                        this.SelectedTreatmentDuration = treatment.Duration;
                                        this.WarningMessage = string.Empty;

                                        if (this._module is not null && this._isCalendarInitialized)
                                        {
                                                string durationStr = treatment.Duration.ToString(format: TIMESPAN_FORMAT);
                                                await this._module.InvokeVoidAsync(
                                                    identifier: JS_SET_DURATION_FUNCTION,
                                                    args: new object[] { durationStr }
                                                );
                                        }
                                }
                        }
                        else
                        {
                                this.SelectedTreatmentIdForBooking = null;
                                this.SelectedTreatmentDuration = null;
                        }
                }

                [JSInvokable]
                public void OnTimeSlotSelected(string start, string end)
                {
                        if (this.SelectedTreatmentIdForBooking is null || this.SelectedTreatmentDuration is null)
                        {
                                this.WarningMessage = "Du skal vælge en behandling i toppen af siden, før du kan markere tid i kalenderen.";
                                this.StateHasChanged();
                                return;
                        }

                        bool isStartValid = DateTime.TryParse(s: start, result: out DateTime startTime);

                        if (!isStartValid)
                        {
                                return;
                        }

                        this.DraggedStartTime = startTime;

                        // We enforce the strict duration of the selected treatment to prevent erroneous drags
                        this.DraggedEndTime = startTime.Add(value: this.SelectedTreatmentDuration.Value);
                        this.IsSmartModalVisible = true;

                        this.StateHasChanged();
                }

                protected void ClearWarning()
                {
                        this.WarningMessage = string.Empty;
                }

                protected async Task AddClinicFilterAsync(ClinicDto clinic)
                {
                        if (clinic is not null && !this.SelectedClinics.Any(predicate: c => c.Id == clinic.Id))
                        {
                                this.SelectedClinics.Add(item: clinic);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemoveClinicFilterAsync(ClinicDto clinicToRemove)
                {
                        if (this.SelectedClinics.RemoveAll(match: c => c.Id == clinicToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task AddRoomFilterAsync(RoomDto room)
                {
                        if (room is not null && !this.SelectedRooms.Any(predicate: r => r.Id == room.Id))
                        {
                                this.SelectedRooms.Add(item: room);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemoveRoomFilterAsync(RoomDto roomToRemove)
                {
                        if (this.SelectedRooms.RemoveAll(match: r => r.Id == roomToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task AddPractitionerFilterAsync(PractitionerLookupDto practitioner)
                {
                        if (practitioner is not null && !this.SelectedPractitioners.Any(predicate: p => p.Id == practitioner.Id))
                        {
                                this.SelectedPractitioners.Add(item: practitioner);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemovePractitionerFilterAsync(PractitionerLookupDto practitionerToRemove)
                {
                        if (this.SelectedPractitioners.RemoveAll(match: p => p.Id == practitionerToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task AddCustomerFilterAsync(CustomerSummaryDto customer)
                {
                        if (customer is not null && !this.SelectedCustomers.Any(predicate: c => c.Id == customer.Id))
                        {
                                this.SelectedCustomers.Add(item: customer);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemoveCustomerFilterAsync(CustomerSummaryDto customerToRemove)
                {
                        if (this.SelectedCustomers.RemoveAll(match: c => c.Id == customerToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

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

                protected void CloseSmartModal()
                {
                        this.IsSmartModalVisible = false;
                        this.DraggedStartTime = null;
                        this.DraggedEndTime = null;
                }

                protected async Task HandleBookingSavedAsync()
                {
                        this.CloseSmartModal();

                        // We reset the selected treatment after a successful booking
                        this.SelectedTreatmentIdForBooking = null;
                        this.SelectedTreatmentDuration = null;

                        await this.RefreshCalendarDataAsync();
                }

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
                                }
                        }

                        this._dotNetRef?.Dispose();
                }
        }
}
