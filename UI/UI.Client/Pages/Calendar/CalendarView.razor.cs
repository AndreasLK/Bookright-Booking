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
                [Inject]
                private Facade.Calendar.ICalendarFacade CalendarFacade { get; set; } = default!;

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

                private DateTime _currentViewStart = DateTime.Today.AddDays(value: -14);
                private DateTime _currentViewEnd = DateTime.Today.AddDays(value: 30);

                private IJSObjectReference? _module;
                private DotNetObjectReference<CalendarView>? _dotNetRef;
                private bool _isCalendarInitialized = false;

                protected override async Task OnInitializedAsync()
                {
                        var lookups = await this.CalendarFacade.GetFilterLookupsAsync();

                        this.AvailableClinics = lookups.Clinics.ToList();
                        this.AvailableRooms = lookups.Rooms.ToList();
                        this.AvailableCustomers = lookups.Customers.ToList();
                        this.AvailablePractitioners = lookups.Practitioners.ToList();
                        this.AvailableTreatments = lookups.Treatments.ToList();

                        await this.RefreshCalendarDataAsync();
                }

                protected override async Task OnAfterRenderAsync(bool firstRender)
                {
                        if (firstRender)
                        {
                                this._dotNetRef = DotNetObjectReference.Create(this);

                                this._module = await this.JS.InvokeAsync<IJSObjectReference>(
                                    identifier: "import",
                                    args: "./Pages/Calendar/CalendarView.razor.js"
                                );

                                await this._module.InvokeVoidAsync(
                                    identifier: "initializeCalendar",
                                    args: new object[] { "calendar-container", this.RenderedCalendarEvents, this._dotNetRef }
                                );

                                this._isCalendarInitialized = true;
                        }
                }

                [JSInvokable]
                public void OnTimeSlotSelected(string start, string end)
                {
                        if (DateTime.TryParse(start, out DateTime startTime) && DateTime.TryParse(end, out DateTime endTime))
                        {
                                this.DraggedStartTime = startTime;
                                this.DraggedEndTime = endTime;
                                this.IsSmartModalVisible = true;
                                this.StateHasChanged();
                        }
                }

                protected async Task AddClinicFilterAsync(ClinicDto clinic)
                {
                        if (clinic is not null && !this.SelectedClinics.Any(c => c.Id == clinic.Id))
                        {
                                this.SelectedClinics.Add(clinic);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemoveClinicFilterAsync(ClinicDto clinicToRemove)
                {
                        if (this.SelectedClinics.RemoveAll(c => c.Id == clinicToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task AddRoomFilterAsync(RoomDto room)
                {
                        if (room is not null && !this.SelectedRooms.Any(r => r.Id == room.Id))
                        {
                                this.SelectedRooms.Add(room);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemoveRoomFilterAsync(RoomDto roomToRemove)
                {
                        if (this.SelectedRooms.RemoveAll(r => r.Id == roomToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task AddPractitionerFilterAsync(PractitionerLookupDto practitioner)
                {
                        if (practitioner is not null && !this.SelectedPractitioners.Any(p => p.Id == practitioner.Id))
                        {
                                this.SelectedPractitioners.Add(practitioner);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemovePractitionerFilterAsync(PractitionerLookupDto practitionerToRemove)
                {
                        if (this.SelectedPractitioners.RemoveAll(p => p.Id == practitionerToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task AddCustomerFilterAsync(CustomerSummaryDto customer)
                {
                        if (customer is not null && !this.SelectedCustomers.Any(c => c.Id == customer.Id))
                        {
                                this.SelectedCustomers.Add(customer);
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                protected async Task RemoveCustomerFilterAsync(CustomerSummaryDto customerToRemove)
                {
                        if (this.SelectedCustomers.RemoveAll(c => c.Id == customerToRemove.Id) > 0)
                        {
                                await this.RefreshCalendarDataAsync();
                                this.StateHasChanged();
                        }
                }

                private async Task RefreshCalendarDataAsync()
                {
                        var clinicIds = this.SelectedClinics.Select(selector: c => c.Id).ToList();
                        var roomIds = this.SelectedRooms.Select(selector: r => r.Id).ToList();
                        var practitionerIds = this.SelectedPractitioners.Select(selector: p => p.Id).ToList();
                        var customerIds = this.SelectedCustomers.Select(selector: c => c.Id).ToList();

                        var updatedEvents = await this.CalendarFacade.RefreshCalendarBookingsAsync(
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
                                    identifier: "updateCalendarEvents",
                                    args: new object[] { "calendar-container", this.RenderedCalendarEvents }
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
