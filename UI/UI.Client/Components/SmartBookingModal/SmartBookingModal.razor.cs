using Facade.Bookings;
using Facade.Clinics;
using Facade.Common.Dtos;
using Facade.Customers;
using Facade.Rooms;
using Facade.Practitioners;
using Facade.Calendar;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UI.Client.Components.SmartBookingModal
{
        /// <summary>
        /// Code-behind for the Smart Booking Modal component, orchestrating the creation of a booking.
        /// </summary>
        public partial class SmartBookingModal : ComponentBase
        {
                /// <summary>
                /// Gets or sets the injected booking facade.
                /// </summary>
                [Inject]
                private IBookingFacade BookingFacade { get; set; } = default!;

                /// <summary>
                /// Gets or sets the start time for the booking.
                /// </summary>
                [Parameter]
                public DateTime StartTime { get; set; }

                /// <summary>
                /// Gets or sets the end time for the booking.
                /// </summary>
                [Parameter]
                public DateTime EndTime { get; set; }

                /// <summary>
                /// Gets or sets the list of available customers.
                /// </summary>
                [Parameter]
                public List<CustomerSummaryDto> AvailableCustomers { get; set; } = new();

                /// <summary>
                /// Gets or sets the list of available clinics.
                /// </summary>
                [Parameter]
                public List<ClinicDto> AvailableClinics { get; set; } = new();

                /// <summary>
                /// Gets or sets the list of available rooms.
                /// </summary>
                [Parameter]
                public List<RoomDto> AvailableRooms { get; set; } = new();

                /// <summary>
                /// Gets or sets the list of available practitioners.
                /// </summary>
                [Parameter]
                public List<PractitionerLookupDto> AvailablePractitioners { get; set; } = new();

                /// <summary>
                /// Gets or sets the list of available treatments.
                /// </summary>
                [Parameter]
                public List<TreatmentLookupDto> AvailableTreatments { get; set; } = new();

                /// <summary>
                /// Gets or sets the callback invoked when the modal is closed.
                /// </summary>
                [Parameter]
                public EventCallback OnClose { get; set; }

                /// <summary>
                /// Gets or sets the callback invoked when a booking is successfully saved.
                /// </summary>
                [Parameter]
                public EventCallback OnBookingSaved { get; set; }

                /// <summary>
                /// Gets the currently selected customer.
                /// </summary>
                public CustomerSummaryDto? SelectedCustomer { get; private set; }

                /// <summary>
                /// Gets the currently selected clinic.
                /// </summary>
                public ClinicDto? SelectedClinic { get; private set; }

                /// <summary>
                /// Gets the currently selected treatment ID.
                /// </summary>
                public Guid? SelectedTreatmentId { get; private set; }

                /// <summary>
                /// Gets the final selected room ID.
                /// </summary>
                public Guid? FinalRoomId { get; private set; }

                /// <summary>
                /// Gets the final selected practitioner ID.
                /// </summary>
                public Guid? FinalPractitionerId { get; private set; }

                /// <summary>
                /// Gets the current system assignment proposal.
                /// </summary>
                public BookingAssignmentFacadeDto? SystemProposal { get; private set; }

                /// <summary>
                /// Gets a value indicating whether the system is currently evaluating a proposal.
                /// </summary>
                public bool IsEvaluatingProposal { get; private set; } = false;

                /// <summary>
                /// Gets the error message to display, if any.
                /// </summary>
                public string ErrorMessage { get; private set; } = string.Empty;

                /// <summary>
                /// Sets the selected clinic and triggers the smart proposal evaluation.
                /// </summary>
                /// <param name="clinic">The selected clinic.</param>
                public void SetClinic(ClinicDto clinic)
                {
                        this.SelectedClinic = clinic;
                        this.TriggerSmartProposalEvaluation();
                }

                /// <summary>
                /// Sets the selected customer and triggers the smart proposal evaluation.
                /// </summary>
                /// <param name="customer">The selected customer.</param>
                public void SetCustomer(CustomerSummaryDto customer)
                {
                        this.SelectedCustomer = customer;
                        this.TriggerSmartProposalEvaluation();
                }

                /// <summary>
                /// Sets the selected treatment based on the UI change event and adjusts EndTime.
                /// </summary>
                /// <param name="e">The change event arguments.</param>
                public void SetTreatment(ChangeEventArgs e)
                {
                        if (e is null || e.Value is null)
                        {
                                return;
                        }

                        string? selectedValue = e.Value.ToString();

                        if (Guid.TryParse(input: selectedValue, result: out Guid treatmentId))
                        {
                                this.SelectedTreatmentId = treatmentId;

                                TreatmentLookupDto? selectedTreatment = this.AvailableTreatments.FirstOrDefault(predicate: t => t.Id == treatmentId);

                                if (selectedTreatment is not null)
                                {
                                        this.EndTime = this.StartTime.Add(value: selectedTreatment.Duration);
                                }

                                this.TriggerSmartProposalEvaluation();
                        }
                }

                /// <summary>
                /// Overrides the automatically proposed room with a manual selection.
                /// </summary>
                /// <param name="room">The manually selected room.</param>
                public void OverrideRoom(RoomDto room)
                {
                        if (room is null)
                        {
                                return;
                        }

                        this.FinalRoomId = room.Id;
                        this.StateHasChanged();
                }

                /// <summary>
                /// Overrides the automatically proposed practitioner with a manual selection.
                /// </summary>
                /// <param name="practitioner">The manually selected practitioner.</param>
                public void OverridePractitioner(PractitionerLookupDto practitioner)
                {
                        if (practitioner is null)
                        {
                                return;
                        }

                        this.FinalPractitionerId = practitioner.Id;
                        this.StateHasChanged();
                }

                /// <summary>
                /// Triggers the async proposal evaluation if all prerequisites are selected.
                /// </summary>
                private void TriggerSmartProposalEvaluation()
                {
                        if (this.SelectedClinic is null || this.SelectedCustomer is null || this.SelectedTreatmentId is null)
                        {
                                return;
                        }

                        _ = this.ResolveProposalAsync();
                }

                /// <summary>
                /// Asynchronously calls the facade to get the best available room and practitioner.
                /// </summary>
                private async Task ResolveProposalAsync()
                {
                        this.IsEvaluatingProposal = true;
                        this.ErrorMessage = string.Empty;

                        await this.InvokeAsync(workItem: this.StateHasChanged);

                        try
                        {
                                this.SystemProposal = await this.BookingFacade.GetAutoAssignmentProposalAsync(
                                    customerId: this.SelectedCustomer!.Id,
                                    treatmentId: this.SelectedTreatmentId!.Value,
                                    clinicId: this.SelectedClinic!.Id,
                                    startDateTime: this.StartTime,
                                    endDateTime: this.EndTime
                                );

                                if (this.SystemProposal is not null && this.SystemProposal.IsSuccessful)
                                {
                                        this.FinalRoomId = this.SystemProposal.ProposedRoomId;
                                        this.FinalPractitionerId = this.SystemProposal.ProposedPractitionerId;
                                }
                        }
                        catch (Exception)
                        {
                                this.ErrorMessage = "Kunne ikke hente anbefaling fra systemet.";
                        }
                        finally
                        {
                                this.IsEvaluatingProposal = false;
                                await this.InvokeAsync(workItem: this.StateHasChanged);
                        }
                }

                /// <summary>
                /// Confirms the booking details and sends the creation request to the facade.
                /// </summary>
                public async Task ConfirmAndSaveBookingAsync()
                {
                        if (this.SelectedCustomer is null || this.SelectedClinic is null || this.SelectedTreatmentId is null)
                        {
                                this.ErrorMessage = "Udfyld venligst kunde, klinik og behandling.";
                                return;
                        }

                        if (this.FinalRoomId is null || this.FinalPractitionerId is null)
                        {
                                this.ErrorMessage = "Et lokale og en behandler skal vælges.";
                                return;
                        }

                        try
                        {
                                await this.BookingFacade.CreateBookingAsync(
                                    customerId: this.SelectedCustomer.Id,
                                    clinicId: this.SelectedClinic.Id,
                                    roomId: this.FinalRoomId.Value,
                                    practitionerId: this.FinalPractitionerId.Value,
                                    treatmentId: this.SelectedTreatmentId.Value,
                                    startDateTime: this.StartTime,
                                    endDateTime: this.EndTime
                                );

                                await this.OnBookingSaved.InvokeAsync();
                        }
                        catch (InvalidOperationException)
                        {
                                this.ErrorMessage = "Det valgte lokale eller behandler er ikke længere ledigt for dette tidspunkt. Vælg venligst en anden.";
                        }
                        catch (Exception)
                        {
                                this.ErrorMessage = "Der opstod en systemfejl under oprettelsen af bookingen.";
                        }
                }

                /// <summary>
                /// Cancels the booking process and closes the modal.
                /// </summary>
                public async Task CancelAsync()
                {
                        await this.OnClose.InvokeAsync();
                }
        }
}
