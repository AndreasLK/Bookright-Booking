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
using System.Threading.Tasks;
using System.Linq;

namespace UI.Client.Components.SmartBookingModal
{
        /// <summary>
        /// Code-behind for the Smart Booking Modal component.
        /// Orchestrates the creation of a booking and fetches detailed price previews.
        /// </summary>
        public partial class SmartBookingModal : ComponentBase
        {
                [Inject]
                private IBookingFacade BookingFacade { get; set; } = default!;

                [Parameter]
                public DateTime StartTime { get; set; }

                [Parameter]
                public DateTime EndTime { get; set; }

                [Parameter]
                public Guid PreselectedTreatmentId { get; set; }

                [Parameter]
                public List<CustomerSummaryDto> AvailableCustomers { get; set; } = new();

                [Parameter]
                public List<ClinicDto> AvailableClinics { get; set; } = new();

                [Parameter]
                public List<RoomDto> AvailableRooms { get; set; } = new();

                [Parameter]
                public List<PractitionerLookupDto> AvailablePractitioners { get; set; } = new();

                [Parameter]
                public List<TreatmentLookupDto> AvailableTreatments { get; set; } = new();

                [Parameter]
                public EventCallback OnClose { get; set; }

                [Parameter]
                public EventCallback OnBookingSaved { get; set; }

                public CustomerSummaryDto? SelectedCustomer { get; private set; }
                public ClinicDto? SelectedClinic { get; private set; }
                public Guid? SelectedTreatmentId { get; private set; }

                public Guid? FinalRoomId { get; private set; }
                public Guid? FinalPractitionerId { get; private set; }

                public BookingAssignmentFacadeDto? SystemProposal { get; private set; }
                public BookingPricingDetailsDto? CalculatedPreviewPrice { get; private set; }

                public bool IsEvaluatingProposal { get; private set; } = false;
                public string ErrorMessage { get; private set; } = string.Empty;

                protected override void OnInitialized()
                {
                        this.SelectedTreatmentId = this.PreselectedTreatmentId;
                }

                public void SetClinic(ClinicDto clinic)
                {
                        this.SelectedClinic = clinic;
                        this.TriggerSmartProposalEvaluation();
                }

                public void SetCustomer(CustomerSummaryDto customer)
                {
                        this.SelectedCustomer = customer;
                        this.TriggerSmartProposalEvaluation();
                }

                public void OverrideRoom(RoomDto room)
                {
                        if (room is null)
                        {
                                return;
                        }

                        this.FinalRoomId = room.Id;
                        this.StateHasChanged();
                }

                public void OverridePractitioner(PractitionerLookupDto practitioner)
                {
                        if (practitioner is null)
                        {
                                return;
                        }

                        this.FinalPractitionerId = practitioner.Id;
                        this.StateHasChanged();
                }

                private void TriggerSmartProposalEvaluation()
                {
                        if (this.SelectedClinic is null || this.SelectedCustomer is null || this.SelectedTreatmentId is null)
                        {
                                return;
                        }

                        _ = this.ResolveProposalAndPriceAsync();
                }

                private async Task ResolveProposalAndPriceAsync()
                {
                        this.IsEvaluatingProposal = true;
                        this.ErrorMessage = string.Empty;

                        await this.InvokeAsync(workItem: this.StateHasChanged);

                        try
                        {
                                this.CalculatedPreviewPrice = await this.BookingFacade.GetBookingPricePreviewAsync(
                                    customerId: this.SelectedCustomer!.Id,
                                    treatmentId: this.SelectedTreatmentId!.Value,
                                    startDateTime: this.StartTime,
                                    endDateTime: this.EndTime
                                );

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
                                this.ErrorMessage = "Kunne ikke hente anbefaling eller prisberegning fra systemet.";
                        }
                        finally
                        {
                                this.IsEvaluatingProposal = false;
                                await this.InvokeAsync(workItem: this.StateHasChanged);
                        }
                }

                public async Task ConfirmAndSaveBookingAsync()
                {
                        if (this.SelectedCustomer is null || this.SelectedClinic is null || this.SelectedTreatmentId is null)
                        {
                                this.ErrorMessage = "Udfyld venligst kunde og klinik.";
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

                public async Task CancelAsync()
                {
                        await this.OnClose.InvokeAsync();
                }
        }
}
