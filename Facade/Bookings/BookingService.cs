using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Facade.Common.Dtos;
using Facade.Common.Extensions;

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
                private readonly ICurrencyConverter _currencyConverter;

                /// <summary>
                /// Initializes a new instance of the <see cref="BookingService"/> class.
                /// </summary>
                /// <param name="bookingRepository">Data store for bookings.</param>
                /// <param name="customerRepository">Data store for customers.</param>
                /// <param name="treatmentRepository">Data store for treatments.</param>
                /// <param name="practitionerRepository">Data store for practitioners.</param>
                /// <param name="clinicRepository">Data store for clinics.</param>
                /// <param name="currencyConverter">Convert payment currency</param>
                public BookingService(
                    IBookingRepository bookingRepository,
                    ICustomerRepository customerRepository,
                    ITreatmentRepository treatmentRepository,
                    IPractitionerRepository practitionerRepository,
                    IClinicRepository clinicRepository,
                    ICurrencyConverter currencyConverter)
                {
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: customerRepository, nameof(customerRepository));
                        ArgumentNullException.ThrowIfNull(argument: treatmentRepository, nameof(treatmentRepository));
                        ArgumentNullException.ThrowIfNull(argument: practitionerRepository, nameof(practitionerRepository));
                        ArgumentNullException.ThrowIfNull(argument: clinicRepository, nameof(clinicRepository));
                        ArgumentNullException.ThrowIfNull(argument: currencyConverter, nameof(currencyConverter));

                        this._bookingRepository = bookingRepository;
                        this._customerRepository = customerRepository;
                        this._treatmentRepository = treatmentRepository;
                        this._practitionerRepository = practitionerRepository;
                        this._clinicRepository = clinicRepository;
                        this._currencyConverter = currencyConverter;
                }

                /// <summary>
                /// Retrieves a paginated, filtered, and sorted list of bookings.
                /// </summary>
                /// <param name="customerId">Optional customer identifier filter.</param>
                /// <param name="clinicId">Optional clinic identifier filter.</param>
                /// <param name="roomId">Optional room identifier filter.</param>
                /// <param name="practitionerId">Optional practitioner identifier filter.</param>
                /// <param name="sortOption">Selected sorting field.</param>
                /// <param name="sortDirection">Direction of the sorting.</param>
                /// <param name="skip">Number of records to bypass.</param>
                /// <param name="take">Maximum number of records to return.</param>
                /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
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

                        var mappingTasks = bookings
                            .Where(b => b.Timeslot != null)
                            .Select(b => this.MapToSummaryDtoAsync(b));

                        var mappedBookings = await Task.WhenAll(mappingTasks);
                        return mappedBookings.ToList();
                }

                /// <summary>
                /// Retrieves a single record by its identifier.
                /// </summary>
                /// <param name="id">The unique identifier.</param>
                /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
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
                /// Reschedules an existing record to a new timeslot.
                /// </summary>
                /// <param name="bookingId">The unique identifier.</param>
                /// <param name="newStartTime">The requested start time.</param>
                /// <param name="newEndTime">The requested end time.</param>
                /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
                public async Task RescheduleBookingAsync(Guid bookingId, DateTime newStartTime, DateTime newEndTime, CancellationToken cancellationToken = default)
                {
                        var booking = await this.GetExistingBookingAsync(bookingId);

                        var newTimeslot = new TimeSlot(newStartTime, newEndTime);
                        booking.Reschedule(newTimeslot);

                        await this._bookingRepository.UpdateAsync(booking);
                }

                /// <summary>
                /// Registers a payment for a booking in the specified currency.
                /// If the currency is not DKK, the amount is fetched as a live rate from CoinGecko
                /// and converted before being stored.
                /// </summary>
                /// <param name="bookingId">The unique identifier of the booking.</param>
                /// <param name="amountInDkk">The monetary amount in DKK.</param>
                /// <param name="currency">The currency the customer wants to pay with.</param>
                /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
                public async Task RegisterPaymentAsync(
                        Guid bookingId,
                        decimal amountInDkk,
                        Currency currency = Currency.DKK,
                        CancellationToken cancellationToken = default)
                {
                        var booking = await this.GetExistingBookingAsync(bookingId);

                        // Fetch the live exchange rate and convert the amount to the chosen currency.
                        // If currency is DKK, GetLiveRateAsync returns 1 — no conversion happens.
                        var rate = await this._currencyConverter.GetLiveRateAsync(
                                fromCurrency: Currency.DKK,
                                toCurrency: currency,
                                ct: cancellationToken);

                        // Multiply by rate: rate = how many units of currency you get per 1 DKK.
                        // Example: 500 DKK * 0.00000163 = 0.000815 BTC
                        var convertedAmount = amountInDkk * rate;

                        var payment = new Money(value: convertedAmount, currency: currency);
                        booking.RegisterPayment(payment);

                        await this._bookingRepository.UpdateAsync(booking);

                        // Payments made in JYD (Jutlandic Dollars) are considered off the books.
                        // The booking is permanently deleted after payment to ensure no record remains.
                        if (currency == Currency.JYD)
                        {
                                await this._bookingRepository.DeleteAsync(booking.Id.Value);
                        }
                }

                /// <summary>
                /// Private helper to fetch an entity and throw an exception if it does not exist.
                /// </summary>
                /// <param name="bookingId">The unique identifier.</param>
                private async Task<Booking> GetExistingBookingAsync(Guid bookingId)
                {
                        var booking = await this._bookingRepository.GetByIdAsync(bookingId);
                        if (booking == null)
                        {
                                throw new InvalidOperationException($"Booking m,ed ID '{bookingId}' kunne ikke findes.");
                        }
                        return booking;
                }

                /// <summary>
                /// Private helper to map the domain entity to a summary object, fetching relationships concurrently.
                /// </summary>
                /// <param name="booking">The domain entity.</param>
                private async Task<BookingSummaryDto> MapToSummaryDtoAsync(Booking booking)
                {
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
                                CustomerName = customer?.ToDisplayFullName() ?? "Ukendt Kunde",
                                PractitionerName = practitioner?.ToDisplayFullName() ?? "Ukendt Behandler",
                                PractitionerId = booking.PractitionerId.Value,
                                TreatmentName = treatment?.Name ?? "Ukendt Behandling",
                                ClinicName = clinic?.Name ?? "Ukendt Klinik",
                                StartTime = booking.Timeslot?.StartDateTime ?? DateTime.MinValue,
                                EndTime = booking.Timeslot?.EndDateTime ?? DateTime.MinValue,
                                AmountPaid = booking.Paid?.Value
                        };
                }

                /// <summary>
                /// Fetches a list of all practitioners for dropdown selection.
                /// </summary>
                /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
                public async Task<IEnumerable<PractitionerLookupDto>> GetAvailablePractitionersAsync(CancellationToken cancellationToken = default)
                {
                        var practitioners = await this._practitionerRepository.GetAllAsync();

                        return practitioners.Select(selector: p => new PractitionerLookupDto
                        {
                                Id = p.Id.Value,
                                DisplayName = p.ToDisplayFullName() ?? "Ukendt Behandler"
                        }).ToList();
                }

                /// <summary>
                /// Reassigns an existing record to a new practitioner.
                /// </summary>
                /// <param name="bookingId">The unique identifier.</param>
                /// <param name="newPractitionerId">The new practitioner identifier.</param>
                /// <param name="cancellationToken">Token to monitor for cancellation requests.</param>
                public async Task ReassignPractitionerAsync(Guid bookingId, Guid newPractitionerId, CancellationToken cancellationToken = default)
                {
                        Booking booking = await this.GetExistingBookingAsync(bookingId: bookingId);
                        PractitionerId newId = new PractitionerId(Value: newPractitionerId);
                        booking.ReassignPractitioner(newPractitionerId: newId);

                        await this._bookingRepository.UpdateAsync(entity: booking);
                }
        }
}
