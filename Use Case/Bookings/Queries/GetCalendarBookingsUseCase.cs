using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                public async Task<IEnumerable<CalendarBookingResultDto>> ExecuteAsync(CalendarBookingFilter filter)
                {
                        ArgumentNullException.ThrowIfNull(argument: filter, paramName: nameof(filter));

                        if (filter.ViewStartDate >= filter.ViewEndDate)
                        {
                                throw new ArgumentException(
                                    message: "The calendar view start date must precede the end date.",
                                    paramName: nameof(filter));
                        }

                        IEnumerable<Guid>? rawClinicIds = filter.ClinicIds?.Select(selector: id => id.Value);
                        IEnumerable<Guid>? rawRoomIds = filter.RoomIds?.Select(selector: id => id.Value);
                        IEnumerable<Guid>? rawPractitionerIds = filter.PractitionerIds?.Select(selector: id => id.Value);
                        IEnumerable<Guid>? rawCustomerIds = filter.CustomerIds?.Select(selector: id => id.Value);

                        CalendarBookingSpecification specification = new CalendarBookingSpecification(
                            viewStartDate: filter.ViewStartDate,
                            viewEndDate: filter.ViewEndDate,
                            clinicIds: rawClinicIds,
                            roomIds: rawRoomIds,
                            practitionerIds: rawPractitionerIds,
                            customerIds: rawCustomerIds
                        );

                        IReadOnlyList<Booking> domainBookings = await this._bookingRepository.FindAsync(specification: specification);

                        if (domainBookings is null || !domainBookings.Any())
                        {
                                return Enumerable.Empty<CalendarBookingResultDto>();
                        }

                        IEnumerable<Task<CalendarBookingResultDto>> mappingTasks = domainBookings.Select(
                            selector: booking => this.MapToResultDtoAsync(booking: booking)
                        );

                        CalendarBookingResultDto[] mappedResults = await Task.WhenAll(tasks: mappingTasks);

                        return mappedResults;
                }

                /// <summary>
                /// Helper method to fetch related external aggregates and construct the raw result DTO.
                /// </summary>
                private async Task<CalendarBookingResultDto> MapToResultDtoAsync(Booking booking)
                {
                        Customer? customer = await this._customerRepository.GetByIdAsync(id: booking.CustomerId.Value);
                        Treatment? treatment = await this._treatmentRepository.GetByIdAsync(id: booking.TreatmentId.Value);

                        string resolvedCustomerName = customer is not null ? customer.ToDisplayFullName() : "Ukendt Kunde";
                        string resolvedTreatmentName = treatment is not null ? treatment.Name : "Ukendt Behandling";
                        bool isPaid = booking.Paid is not null;

                        return new CalendarBookingResultDto(
                            BookingId: booking.Id,
                            ClinicId: booking.ClinicId,
                            TreatmentName: resolvedTreatmentName,
                            CustomerName: resolvedCustomerName,
                            StartTime: booking.Timeslot.StartDateTime,
                            EndTime: booking.Timeslot.EndDateTime,
                            IsPaid: isPaid
                        );
                }
        }
}
