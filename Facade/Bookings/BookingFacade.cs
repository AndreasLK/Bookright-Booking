using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using System;
using System.Threading.Tasks;
using Use_Case.BestDiscount;
using Use_Case.Bookings.Commands;
using Use_Case.Bookings.Queries;

namespace Facade.Bookings
{
        public class BookingFacade : IBookingFacade
        {
                private readonly GetAutoAssignmentProposalUseCase _getAutoAssignmentProposalUseCase;
                private readonly CreateBookingUseCase _createBookingUseCase;
                private readonly PayBookingUseCase _payBookingUseCase;
                private readonly PricingService _pricingService;
                private readonly IBookingRepository _bookingRepository;

                public BookingFacade(
                    GetAutoAssignmentProposalUseCase getAutoAssignmentProposalUseCase,
                    CreateBookingUseCase createBookingUseCase,
                    PayBookingUseCase payBookingUseCase,
                    PricingService pricingService,
                    IBookingRepository bookingRepository)
                {
                        ArgumentNullException.ThrowIfNull(argument: getAutoAssignmentProposalUseCase, paramName: nameof(getAutoAssignmentProposalUseCase));
                        ArgumentNullException.ThrowIfNull(argument: createBookingUseCase, paramName: nameof(createBookingUseCase));
                        ArgumentNullException.ThrowIfNull(argument: payBookingUseCase, paramName: nameof(payBookingUseCase));
                        ArgumentNullException.ThrowIfNull(argument: pricingService, paramName: nameof(pricingService));
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, paramName: nameof(bookingRepository));

                        this._getAutoAssignmentProposalUseCase = getAutoAssignmentProposalUseCase;
                        this._createBookingUseCase = createBookingUseCase;
                        this._payBookingUseCase = payBookingUseCase;
                        this._pricingService = pricingService;
                        this._bookingRepository = bookingRepository;
                }

                public async Task<BookingAssignmentFacadeDto> GetAutoAssignmentProposalAsync(Guid customerId, Guid treatmentId, Guid clinicId, DateTime startDateTime, DateTime endDateTime)
                {
                        if (startDateTime >= endDateTime) throw new ArgumentException(message: "Start time must precede end time.");

                        TimeSlot requestedTimeSlot = new TimeSlot(startDateTime: startDateTime, endDateTime: endDateTime);

                        BookingAssignmentProposalDto proposal = await this._getAutoAssignmentProposalUseCase.ExecuteAsync(
                            customerId: customerId, treatmentId: treatmentId, clinicId: clinicId, requestedTimeSlot: requestedTimeSlot);

                        return new BookingAssignmentFacadeDto(
                            ProposedRoomId: proposal.ProposedRoomId, ProposedRoomName: proposal.ProposedRoomName,
                            ProposedPractitionerId: proposal.ProposedPractitionerId, ProposedPractitionerName: proposal.ProposedPractitionerName,
                            IsSuccessful: proposal.IsSuccessful);
                }

                public async Task<Guid> CreateBookingAsync(Guid customerId, Guid clinicId, Guid roomId, Guid practitionerId, Guid treatmentId, DateTime startDateTime, DateTime endDateTime)
                {
                        CreateBookingCommand command = new CreateBookingCommand(
                            CustomerId: customerId, ClinicId: clinicId, RoomId: roomId, PractitionerId: practitionerId,
                            TreatmentId: treatmentId, StartDateTime: startDateTime, EndDateTime: endDateTime);

                        return await this._createBookingUseCase.ExecuteAsync(command: command);
                }

                public async Task MarkBookingAsPaidAsync(Guid bookingId, Currency currency)
                {
                        await this._payBookingUseCase.ExecuteAsync(bookingIdRaw: bookingId, currency: currency);
                }

                public async Task<BookingPricingDetailsDto> GetBookingPricePreviewAsync(Guid customerId, Guid treatmentId, DateTime startDateTime, DateTime endDateTime)
                {
                        CustomerId cId = new CustomerId(Value: customerId);
                        TreatmentId tId = new TreatmentId(Value: treatmentId);
                        TimeSlot ts = new TimeSlot(startDateTime: startDateTime, endDateTime: endDateTime);

                        PricingBreakdown breakdown = await this._pricingService.GetPreviewPriceAsync(customerId: cId, treatmentId: tId, timeslot: ts);

                        return new BookingPricingDetailsDto(
                            BasePrice: breakdown.BasePrice.Value,
                            SurchargeAmount: breakdown.SurchargeAmount.Value,
                            SurchargeReason: breakdown.SurchargeReason,
                            DiscountAmount: breakdown.DiscountAmount.Value,
                            DiscountReason: breakdown.DiscountReason,
                            EvaluatedDiscounts: breakdown.EvaluatedDiscounts,
                            FinalPrice: breakdown.FinalPrice.Value
                        );
                }

                public async Task<BookingPricingDetailsDto> GetSavedBookingPriceAsync(Guid bookingId)
                {
                        BookingId bId = new BookingId(Value: bookingId);
                        Booking? booking = await this._bookingRepository.GetByIdAsync(id: bId.Value);
                        if (booking is null) throw new InvalidOperationException(message: "Booking not found");

                        PricingBreakdown breakdown = await this._pricingService.GetFinalPriceAsync(customerId: booking.CustomerId, bookingId: bId);

                        return new BookingPricingDetailsDto(
                            BasePrice: breakdown.BasePrice.Value,
                            SurchargeAmount: breakdown.SurchargeAmount.Value,
                            SurchargeReason: breakdown.SurchargeReason,
                            DiscountAmount: breakdown.DiscountAmount.Value,
                            DiscountReason: breakdown.DiscountReason,
                            EvaluatedDiscounts: breakdown.EvaluatedDiscounts,
                            FinalPrice: breakdown.FinalPrice.Value
                        );
                }
        }
}
