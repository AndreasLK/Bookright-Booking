using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Facade.Common.Dtos;
using Facade.Common.Extensions;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Specifications.Customers;

namespace Facade.Bookings
{
        /// <summary>
        /// Orchestrates booking operations between the UI and domain layers.
        /// </summary>
        public class BookingService
        {
                private readonly IBookingRepository _bookingRepository;
                private readonly ICustomerRepository _customerRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly IPractitionerRepository _practitionerRepository;
                private readonly IClinicRepository _clinicRepository;

                /// <summary>
                /// Initializes a new instance of the BookingService class.
                /// </summary>
                public BookingService(
                    IBookingRepository bookingRepository,
                    ICustomerRepository customerRepository,
                    ITreatmentRepository treatmentRepository,
                    IPractitionerRepository practitionerRepository,
                    IClinicRepository clinicRepository)
                {
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, paramName: nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: customerRepository, paramName: nameof(customerRepository));
                        ArgumentNullException.ThrowIfNull(argument: treatmentRepository, paramName: nameof(treatmentRepository));
                        ArgumentNullException.ThrowIfNull(argument: practitionerRepository, paramName: nameof(practitionerRepository));
                        ArgumentNullException.ThrowIfNull(argument: clinicRepository, paramName: nameof(clinicRepository));

                        this._bookingRepository = bookingRepository;
                        this._customerRepository = customerRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._practitionerRepository = practitionerRepository;
                        this._clinicRepository = clinicRepository;
                }

                /// <summary>
                /// Retrieves a paginated, filtered, and sorted list of bookings.
                /// </summary>
                public async Task<IEnumerable<BookingSummaryDto>> GetBookingsAsync(
                    Guid? customerId = null,
                    Guid? clinicId = null,
                    Guid? roomId = null,
                    Guid? practitionerId = null,
                    BookingSortOption sortOption = BookingSortOption.StartTime,
                    SortDirection sortDirection = SortDirection.Ascending,
                    int skip = 0,
                    int take = 100,
                    CancellationToken cancellationToken = default)
                {
                        BookingSearchSpecification specification = new BookingSearchSpecification(
                            customerId: customerId,
                            clinicId: clinicId,
                            roomId: roomId,
                            practitionerId: practitionerId,
                            sortOption: sortOption,
                            sortDirection: sortDirection,
                            skip: skip,
                            take: take
                        );

                        IReadOnlyList<Booking> bookings = await this._bookingRepository.FindAsync(specification: specification);

                        if (bookings is null || !bookings.Any())
                        {
                                return Enumerable.Empty<BookingSummaryDto>();
                        }

                        IEnumerable<Task<BookingSummaryDto>> mappingTasks = bookings
                            .Where(predicate: b => b.Timeslot is not null)
                            .Select(selector: b => this.MapToSummaryDtoAsync(booking: b));

                        BookingSummaryDto[] mappedBookings = await Task.WhenAll(tasks: mappingTasks);

                        return mappedBookings.ToList();
                }

                /// <summary>
                /// Retrieves a single record by its identifier.
                /// </summary>
                public async Task<BookingSummaryDto?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default)
                {
                        Booking? booking = await this._bookingRepository.GetByIdAsync(id: id);

                        if (booking is null)
                        {
                                return null;
                        }

                        return await this.MapToSummaryDtoAsync(booking: booking);
                }

                /// <summary>
                /// Reschedules an existing record to a new timeslot.
                /// </summary>
                public async Task RescheduleBookingAsync(Guid bookingId, DateTime newStartTime, DateTime newEndTime, CancellationToken cancellationToken = default)
                {
                        Booking booking = await this.GetExistingBookingAsync(bookingId: bookingId);

                        TimeSlot newTimeslot = new TimeSlot(
                            startDateTime: newStartTime,
                            endDateTime: newEndTime
                        );

                        // Assuming the parameter for Reschedule is 'newTimeslot' or 'timeSlot'. Update parameter name if your Domain entity differs.
                        booking.Reschedule(newTimeslot: newTimeslot);

                        await this._bookingRepository.UpdateAsync(entity: booking);
                }

                /// <summary>
                /// Registers a payment.
                /// </summary>
                public async Task RegisterPaymentAsync(Guid bookingId, decimal amountPaid, CancellationToken cancellationToken = default)
                {
                        Booking booking = await this.GetExistingBookingAsync(bookingId: bookingId);

                        Money payment = new Money(
                            value: amountPaid,
                            currency: Currency.DKK
                        );

                        booking.RegisterPayment(payment: payment);

                        await this._bookingRepository.UpdateAsync(entity: booking);
                }

                /// <summary>
                /// Helper to fetch an entity and throw an exception if it does not exist.
                /// </summary>
                private async Task<Booking> GetExistingBookingAsync(Guid bookingId)
                {
                        Booking? booking = await this._bookingRepository.GetByIdAsync(id: bookingId);

                        if (booking is null)
                        {
                                throw new InvalidOperationException(message: $"Booking med ID '{bookingId}' kunne ikke findes.");
                        }

                        return booking;
                }

                /// <summary>
                /// Helper to map the domain entity to a summary object, fetching relationships concurrently.
                /// </summary>
                private async Task<BookingSummaryDto> MapToSummaryDtoAsync(Booking booking)
                {
                        if (booking is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(booking));
                        }

                        Task<Customer?> customerTask = this._customerRepository.GetByIdAsync(id: booking.CustomerId.Value);
                        Task<Treatment?> treatmentTask = this._treatmentRepository.GetByIdAsync(id: booking.TreatmentId.Value);
                        Task<Practitioner?> practitionerTask = this._practitionerRepository.GetByIdAsync(id: booking.PractitionerId.Value);
                        Task<Clinic?> clinicTask = this._clinicRepository.GetByIdAsync(id: booking.ClinicId.Value);

                        await Task.WhenAll(tasks: new Task[] { customerTask, treatmentTask, practitionerTask, clinicTask });

                        Customer? customer = await customerTask;
                        Treatment? treatment = await treatmentTask;
                        Practitioner? practitioner = await practitionerTask;
                        Clinic? clinic = await clinicTask;

                        return new BookingSummaryDto
                        {
                                Id = booking.Id.Value,
                                CustomerId = booking.CustomerId.Value,
                                CustomerName = customer is not null ? customer.ToDisplayFullName() : "Ukendt Kunde",
                                PractitionerName = practitioner is not null ? practitioner.ToDisplayFullName() : "Ukendt Behandler",
                                PractitionerId = booking.PractitionerId.Value,
                                TreatmentName = treatment is not null ? treatment.Name : "Ukendt Behandling",
                                ClinicName = clinic is not null ? clinic.Name : "Ukendt Klinik",
                                StartTime = booking.Timeslot is not null ? booking.Timeslot.StartDateTime : DateTime.MinValue,
                                EndTime = booking.Timeslot is not null ? booking.Timeslot.EndDateTime : DateTime.MinValue,
                                AmountPaid = booking.Paid is not null ? booking.Paid.Value : null
                        };
                }

                /// <summary>
                /// Fetches a list of all practitioners for dropdown selection.
                /// </summary>
                public async Task<IEnumerable<PractitionerLookupDto>> GetAvailablePractitionersAsync(CancellationToken cancellationToken = default)
                {
                        IReadOnlyList<Practitioner> practitioners = await this._practitionerRepository.GetAllAsync();

                        IEnumerable<PractitionerLookupDto> dtos = practitioners.Select(selector: p => new PractitionerLookupDto
                        {
                                Id = p.Id.Value,
                                DisplayName = p.ToDisplayFullName() ?? "Ukendt Behandler"
                        });

                        return dtos.ToList();
                }

                /// <summary>
                /// Reassigns an existing record to a new practitioner.
                /// </summary>
                public async Task ReassignPractitionerAsync(Guid bookingId, Guid newPractitionerId, CancellationToken cancellationToken = default)
                {
                        Booking booking = await this.GetExistingBookingAsync(bookingId: bookingId);

                        PractitionerId newId = new PractitionerId(Value: newPractitionerId);
                        booking.ReassignPractitioner(newPractitionerId: newId);

                        await this._bookingRepository.UpdateAsync(entity: booking);
                }
        }
}
