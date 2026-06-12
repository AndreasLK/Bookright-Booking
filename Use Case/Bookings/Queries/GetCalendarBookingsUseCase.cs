using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Use_Case.Bookings.Queries
{
        /// <summary>
        /// Use case for retrieving raw booking data for the calendar view.
        /// Uses the Specification pattern to fetch data supporting multiple filter selections (Clinics, Rooms, etc.).
        /// </summary>
        public class GetCalendarBookingsUseCase : IGetCalendarBookingsUseCase
        {
                private readonly IBookingRepository _bookingRepository;
                private readonly ICustomerRepository _customerRepository;
                private readonly ITreatmentRepository _treatmentRepository;

                /// <summary>
                /// Initializes a new instance of the GetCalendarBookingsUseCase.
                /// </summary>
                /// <param name="bookingRepository">Data store for bookings.</param>
                /// <param name="customerRepository">Data store for resolving customer details.</param>
                /// <param name="treatmentRepository">Data store for resolving treatment details.</param>
                public GetCalendarBookingsUseCase(
                    IBookingRepository bookingRepository,
                    ICustomerRepository customerRepository,
                    ITreatmentRepository treatmentRepository)
                {
                        ArgumentNullException.ThrowIfNull(argument: bookingRepository, paramName: nameof(bookingRepository));
                        ArgumentNullException.ThrowIfNull(argument: customerRepository, paramName: nameof(customerRepository));
                        ArgumentNullException.ThrowIfNull(argument: treatmentRepository, paramName: nameof(treatmentRepository));

                        this._bookingRepository = bookingRepository;
                        this._customerRepository = customerRepository;
                        this._treatmentRepository = treatmentRepository;
                }

                /// <summary>
                /// Executes the query to fetch raw calendar bookings using the multi-select specification pattern.
                /// </summary>
                /// <param name="filter">The filtering criteria for the calendar, supporting multiple selections.</param>
                /// <returns>A collection of raw data DTOs.</returns>
                public async Task<IEnumerable<CalendarBookingResultDto>> ExecuteAsync(CalendarBookingFilter filter)
                {
                        ArgumentNullException.ThrowIfNull(argument: filter, paramName: nameof(filter));

                        if (filter.ViewStartDate >= filter.ViewEndDate)
                        {
                                throw new ArgumentException(
                                    message: "The calendar view start date must precede the end date.",
                                    paramName: nameof(filter));
                        }

                        // 1. Extract the raw Guids from the Strongly Typed IDs for the EF Core Specification
                        IEnumerable<Guid>? rawClinicIds = filter.ClinicIds?.Select(selector: id => id.Value);
                        IEnumerable<Guid>? rawRoomIds = filter.RoomIds?.Select(selector: id => id.Value);
                        IEnumerable<Guid>? rawPractitionerIds = filter.PractitionerIds?.Select(selector: id => id.Value);
                        IEnumerable<Guid>? rawCustomerIds = filter.CustomerIds?.Select(selector: id => id.Value);

                        // 2. Build the specification translating the filter to Domain rules
                        CalendarBookingSpecification specification = new CalendarBookingSpecification(
                            viewStartDate: filter.ViewStartDate,
                            viewEndDate: filter.ViewEndDate,
                            clinicIds: rawClinicIds,
                            roomIds: rawRoomIds,
                            practitionerIds: rawPractitionerIds,
                            customerIds: rawCustomerIds
                        );

                        // 3. Fetch via the generic Repository interface
                        IReadOnlyList<Booking> domainBookings = await this._bookingRepository.FindAsync(specification: specification);

                        if (domainBookings == null || !domainBookings.Any())
                        {
                                return Enumerable.Empty<CalendarBookingResultDto>();
                        }

                        // 4. Map results concurrently to resolve aggregate roots (Customer, Treatment) avoiding N+1 blocking
                        IEnumerable<Task<CalendarBookingResultDto>> mappingTasks = domainBookings.Select(
                            selector: booking => this.MapToResultDtoAsync(booking: booking)
                        );

                        CalendarBookingResultDto[] mappedResults = await Task.WhenAll(tasks: mappingTasks);

                        return mappedResults;
                }

                /// <summary>
                /// Helper method to fetch related external aggregates and construct the raw result DTO.
                /// </summary>
                /// <param name="booking">The domain booking entity containing the strongly typed IDs.</param>
                /// <returns>The fully resolved DTO.</returns>
                private async Task<CalendarBookingResultDto> MapToResultDtoAsync(Booking booking)
                {
                        Customer? customer = await this._customerRepository.GetByIdAsync(id: booking.CustomerId.Value);
                        Treatment? treatment = await this._treatmentRepository.GetByIdAsync(id: booking.TreatmentId.Value);

                        // Utilizing the extension method on the Person base class to get the preferred name
                        string resolvedCustomerName = customer != null ? customer.ToDisplayFullName() : "Ukendt Kunde";
                        string resolvedTreatmentName = treatment != null ? treatment.Name : "Ukendt Behandling";

                        return new CalendarBookingResultDto(
                            BookingId: booking.Id,
                            ClinicId: booking.ClinicId,
                            TreatmentName: resolvedTreatmentName,
                            CustomerName: resolvedCustomerName,
                            StartTime: booking.Timeslot.StartDateTime,
                            EndTime: booking.Timeslot.EndDateTime
                        );
                }
        }
}
