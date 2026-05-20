using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Facade.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facade.Bookings
{
        public class BookingService
        {
                private readonly IBookingRepository _bookingRepository;
                private readonly ICustomerRepository _customerRepository;
                private readonly ITreatmentRepository _treatmentRepository;
                private readonly IPractitionerRepository _practitionerRepository;
                private readonly IClinicRepository _clinicRepository;

                public BookingService(
                    IBookingRepository bookingRepository,
                    ICustomerRepository customerRepository,
                    ITreatmentRepository treatmentRepository,
                    IPractitionerRepository practitionerRepository,
                    IClinicRepository clinicRepository)
                {
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: customerRepository, nameof(customerRepository));
                        ArgumentNullException.ThrowIfNull(argument: treatmentRepository, nameof(treatmentRepository));
                        ArgumentNullException.ThrowIfNull(argument: practitionerRepository, nameof(practitionerRepository));
                        ArgumentNullException.ThrowIfNull(argument: clinicRepository, nameof(clinicRepository));

                        this._bookingRepository = bookingRepository;
                        this._customerRepository = customerRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._practitionerRepository = practitionerRepository;
                        this._clinicRepository = clinicRepository;
                }

                /// <summary>
                /// Retrieves a paginated, filtered, and sorted list of bookings and maps them for the UI.
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
                        var specification = new BookingSearchSpecification(
                            customerId, clinicId, roomId, practitionerId, sortOption, sortDirection, skip, take);

                        var bookings = await this._bookingRepository.FindAsync(specification);
                        if (bookings == null || !bookings.Any())
                        {
                                return Enumerable.Empty<BookingSummaryDto>();
                        }

                        // Optimization: Process the mapping tasks concurrently instead of awaiting sequentially in a loop
                        var mappingTasks = bookings
                            .Where(b => b.Timeslot != null)
                            .Select(b => this.MapToSummaryDtoAsync(b));

                        var mappedBookings = await Task.WhenAll(mappingTasks);
                        return mappedBookings.ToList();
                }

                /// <summary>
                /// Retrieves a single booking by its ID and maps it to a summary Data Transfer Object.
                /// </summary>
                public async Task<BookingSummaryDto?> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken = default)
                {
                        var booking = await this._bookingRepository.GetByIdAsync(id);
                        if (booking == null)
                        {
                                return null;
                        }

                        return await this.MapToSummaryDtoAsync(booking);
                }

                /// <summary>
                /// Reschedules an existing booking to a new timeslot.
                /// </summary>
                public async Task RescheduleBookingAsync(Guid bookingId, DateTime newStartTime, DateTime newEndTime, CancellationToken cancellationToken = default)
                {
                        var booking = await this.GetExistingBookingAsync(bookingId);

                        var newTimeslot = new TimeSlot(newStartTime, newEndTime);

                        // Execute Domain Behavior on the Entity
                        booking.Reschedule(newTimeslot);

                        await this._bookingRepository.UpdateAsync(booking);
                }

                /// <summary>
                /// Registers a payment against an existing booking.
                /// </summary>
                public async Task RegisterPaymentAsync(Guid bookingId, decimal amountPaid, CancellationToken cancellationToken = default)
                {
                        var booking = await this.GetExistingBookingAsync(bookingId);

                        // Execute Domain Behavior on the Entity
                        var payment = new Money(amountPaid, Currency.DKK);
                        booking.RegisterPayment(payment);

                        await this._bookingRepository.UpdateAsync(booking);
                }

                /// <summary>
                /// Private helper to fetch an entity and throw a standardized exception if it does not exist.
                /// </summary>
                private async Task<Booking> GetExistingBookingAsync(Guid bookingId)
                {
                        var booking = await this._bookingRepository.GetByIdAsync(bookingId);
                        if (booking == null)
                        {
                                throw new InvalidOperationException($"Booking with ID '{bookingId}' was not found.");
                        }
                        return booking;
                }

                /// <summary>
                /// Private helper to map a Booking Entity to a Summary DTO, fetching relationships concurrently.
                /// </summary>
                private async Task<BookingSummaryDto> MapToSummaryDtoAsync(Booking booking)
                {
                        // Note: If migrating to Entity Framework Core later, configure the repository 
                        // to use .Include() to avoid manual fetching. For now, we fetch concurrently to avoid N+1 bottlenecks.
                        var customerTask = this._customerRepository.GetByIdAsync(booking.CustomerId.Value);
                        var treatmentTask = this._treatmentRepository.GetByIdAsync(booking.TreatmentId.Value);
                        var practitionerTask = this._practitionerRepository.GetByIdAsync(booking.PractitionerId.Value);
                        var clinicTask = this._clinicRepository.GetByIdAsync(booking.ClinicId.Value);

                        await Task.WhenAll(customerTask, treatmentTask, practitionerTask, clinicTask);

                        var customer = await customerTask;
                        var treatment = await treatmentTask;
                        var practitioner = await practitionerTask;
                        var clinic = await clinicTask;

                        return new BookingSummaryDto
                        {
                                Id = booking.Id.Value,
                                CustomerId = booking.CustomerId.Value,
                                CustomerName = customer?.ToDisplayFullName() ?? "Unknown Customer",
                                PractitionerName = practitioner?.ToDisplayFullName() ?? "Unknown Practitioner",
                                TreatmentName = treatment?.Name ?? "Unknown Treatment",
                                ClinicName = clinic?.Name ?? "Unknown Clinic",
                                StartTime = booking.Timeslot?.StartDateTime ?? DateTime.MinValue,
                                EndTime = booking.Timeslot?.EndDateTime ?? DateTime.MinValue,
                                AmountPaid = booking.Paid?.Value
                        };
                }
        }
}
