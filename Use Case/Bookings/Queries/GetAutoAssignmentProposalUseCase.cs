using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Services;
using Domain.Specifications;
using Domain.Value_Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Use_Case.Bookings.Queries
{
        /// <summary>
        /// Orchestrates the automatic selection of an optimal room and practitioner 
        /// for a new booking based on availability and domain rules.
        /// </summary>
        public class GetAutoAssignmentProposalUseCase
        {
                private readonly ICustomerRepository _customerRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly IClinicRepository _clinicRepository;
                private readonly IBookingRepository _bookingRepository;
                private readonly IRoomRepository _roomRepository;
                private readonly IPractitionerRepository _practitionerRepository;
                private readonly BookingAssignmentDomainService _domainService;

                public GetAutoAssignmentProposalUseCase(
                    ICustomerRepository customerRepository,
                    ITreatmentRepository treatmentRepository,
                    IClinicRepository clinicRepository,
                    IBookingRepository bookingRepository,
                    IRoomRepository roomRepository,
                    IPractitionerRepository practitionerRepository,
                    BookingAssignmentDomainService domainService)
                {
                        ArgumentNullException.ThrowIfNull(argument: customerRepository, paramName: nameof(customerRepository));
                        ArgumentNullException.ThrowIfNull(argument: treatmentRepository, paramName: nameof(treatmentRepository));
                        ArgumentNullException.ThrowIfNull(argument: clinicRepository, paramName: nameof(clinicRepository));
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, paramName: nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: roomRepository, paramName: nameof(roomRepository));
                        ArgumentNullException.ThrowIfNull(argument: practitionerRepository, paramName: nameof(practitionerRepository));
                        ArgumentNullException.ThrowIfNull(argument: domainService, paramName: nameof(domainService));

                        this._customerRepository = customerRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._clinicRepository = clinicRepository;
                        this._bookingRepository = bookingRepository;
                        this._roomRepository = roomRepository;
                        this._practitionerRepository = practitionerRepository;
                        this._domainService = domainService;
                }

                /// <summary>
                /// Executes the query to find the best available room and practitioner.
                /// </summary>
                public async Task<BookingAssignmentProposalDto> ExecuteAsync(
                    Guid customerId,
                    Guid treatmentId,
                    Guid clinicId,
                    TimeSlot requestedTimeSlot)
                {
                        if (requestedTimeSlot is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(requestedTimeSlot));
                        }

                        Customer? customer = await this._customerRepository.GetByIdAsync(id: customerId);
                        Treatment? treatment = await this._treatmentRepository.GetByIdAsync(id: treatmentId);
                        Clinic? clinic = await this._clinicRepository.GetByIdAsync(id: clinicId);

                        if (customer is null || treatment is null || clinic is null)
                        {
                                return new BookingAssignmentProposalDto(
                                    ProposedRoomId: null, ProposedRoomName: null,
                                    ProposedPractitionerId: null, ProposedPractitionerName: null,
                                    IsSuccessful: false
                                );
                        }

                        // 1. Fetch overlapping bookings for the selected timeslot
                        OverlappingBookingsSpecification overlapSpec = new OverlappingBookingsSpecification(
                            requestedTimeSlot: requestedTimeSlot
                        );

                        IReadOnlyList<Booking> overlappingBookings = await this._bookingRepository.FindAsync(specification: overlapSpec);

                        // 2. Extract strictly booked IDs
                        IEnumerable<Guid> bookedRoomIds = overlappingBookings.Select(selector: b => b.RoomId.Value);
                        IEnumerable<Guid> bookedPractitionerIds = overlappingBookings.Select(selector: b => b.PractitionerId.Value);

                        // 3. Resolve actual available entities using Specifications
                        IReadOnlyList<Room> availableRooms = await this.ResolveAvailableRoomsAsync(
                            clinic: clinic,
                            bookedRoomIds: bookedRoomIds
                        );

                        IReadOnlyList<Practitioner> availablePractitioners = await this.ResolveAvailablePractitionersAsync(
                            bookedPractitionerIds: bookedPractitionerIds
                        );

                        // 4. Delegate to the Pure Domain Service to enforce business rules (Gender preferences, Primary Room rules)
                        Room? bestRoom = this._domainService.SelectBestRoom(
                            availableRooms: availableRooms,
                            categoryId: treatment.CategoryId
                        );

                        Practitioner? bestPractitioner = this._domainService.SelectBestPractitioner(
                            availablePractitioners: availablePractitioners,
                            customer: customer
                        );

                        bool isSuccess = bestRoom is not null && bestPractitioner is not null;

                        return new BookingAssignmentProposalDto(
                            ProposedRoomId: bestRoom?.Id.Value,
                            ProposedRoomName: bestRoom?.Name,
                            ProposedPractitionerId: bestPractitioner?.Id.Value,
                            ProposedPractitionerName: bestPractitioner?.ToDisplayFullName(),
                            IsSuccessful: isSuccess
                        );
                }

                /// <summary>
                /// Orchestrates the filtering of rooms by checking the clinic's intrinsic capacity against current bookings.
                /// </summary>
                private async Task<IReadOnlyList<Room>> ResolveAvailableRoomsAsync(Clinic clinic, IEnumerable<Guid> bookedRoomIds)
                {
                        IEnumerable<Guid> availableRoomIds = clinic.RoomIds
                            .Select(selector: r => r.Value)
                            .Except(second: bookedRoomIds);

                        AvailableRoomsSpecification roomSpec = new AvailableRoomsSpecification(availableRoomIds: availableRoomIds);

                        IReadOnlyList<Room> rooms = await this._roomRepository.FindAsync(specification: roomSpec);

                        return rooms;
                }

                /// <summary>
                /// Retrieves the collection of practitioners who do not possess a conflicting booking.
                /// </summary>
                private async Task<IReadOnlyList<Practitioner>> ResolveAvailablePractitionersAsync(IEnumerable<Guid> bookedPractitionerIds)
                {
                        AvailablePractitionersSpecification practitionerSpec = new AvailablePractitionersSpecification(
                            bookedPractitionerIds: bookedPractitionerIds
                        );

                        IReadOnlyList<Practitioner> practitioners = await this._practitionerRepository.FindAsync(specification: practitionerSpec);

                        return practitioners;
                }
        }
}
