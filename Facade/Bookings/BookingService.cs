using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using Facade.Common.Extensions;
using Domain.Entities;
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
                        int take = 100
                    )
                {
                        var specification = new BookingSearchSpecification(
                                customerId: customerId,
                                clinicId: clinicId,
                                roomId: roomId,
                                practitionerId: practitionerId,
                                sortOption: sortOption,
                                sortDirection: sortDirection,
                                skip: skip,
                                take: take
                            );

                        var bookings = await this._bookingRepository.FindAsync(specification);
                        if (bookings is null || !bookings.Any())
                        {
                                return Enumerable.Empty<BookingSummaryDto>();
                        }

                        var result = new List<BookingSummaryDto>();


                        foreach (var booking in bookings)
                        {
                                if (booking.Timeslot is null)
                                {
                                        continue;
                                }

                                var customer = await this._customerRepository.GetByIdAsync(booking.CustomerId.Value);
                                var treatment = await this._treatmentRepository.GetByIdAsync(booking.TreatmentId.Value);
                                var practitioner = await this._practitionerRepository.GetByIdAsync(booking.PractitionerId.Value);
                                var clinic = await this._clinicRepository.GetByIdAsync(booking.ClinicId.Value);


                                string mappedCustomerName = customer != null ? customer.ToDisplayFullName() : "Unknown Customer";
                                string mappedPractitionerName = practitioner != null ? practitioner.ToDisplayFullName() : "Unknown Practitioner";
                                string mappedTreatmentName = treatment != null ? treatment.Name : "Unknown Treatment";
                                string mappedClinicName = clinic != null ? clinic.Name : "Unknown Clinic";


                                result.Add(new BookingSummaryDto
                                {
                                        Id = booking.Id.Value,
                                        CustomerId = booking.CustomerId.Value,
                                        CustomerName = mappedCustomerName,
                                        PractitionerName = mappedPractitionerName,
                                        TreatmentName = mappedTreatmentName,
                                        ClinicName = mappedClinicName,
                                        StartTime = booking.Timeslot.StartDateTime,
                                        EndTime = booking.Timeslot.EndDateTime,
                                        AmountPaid = booking.Paid?.Value,
                                });

                        }

                        return result;
                }
        }
}
