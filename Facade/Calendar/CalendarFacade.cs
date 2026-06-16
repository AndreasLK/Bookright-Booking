using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facade.Clinics;
using Facade.Common.Dtos;
using Facade.Customers;
using Facade.Rooms;
using Facade.Practitioners;
using Use_Case.Bookings.Queries;
using Domain.Interfaces.Repositories;

namespace Facade.Calendar
{
        public class CalendarFacade : ICalendarFacade
        {
                private readonly IGetCalendarBookingsUseCase _getCalendarBookingsUseCase;
                private readonly IClinicRepository _clinicRepository;
                private readonly IRoomRepository _roomRepository;
                private readonly IPractitionerRepository _practitionerRepository;
                private readonly ICustomerRepository _customerRepository;
                private readonly ITreatmentRepository _treatmentRepository;

                private const string DEFAULT_HEX_COLOR = "#3788D8";
                private const string HEX_FORMAT = "X2";
                private const string COLOR_PAID_SUCCESS = "#198754";
                private const string COLOR_UNPAID_PENDING = "#0dcaf0";

                public CalendarFacade(
                    IGetCalendarBookingsUseCase getCalendarBookingsUseCase,
                    IClinicRepository clinicRepository,
                    IRoomRepository roomRepository,
                    IPractitionerRepository practitionerRepository,
                    ICustomerRepository customerRepository,
                    ITreatmentRepository treatmentRepository)
                {
                        this._getCalendarBookingsUseCase = getCalendarBookingsUseCase;
                        this._clinicRepository = clinicRepository;
                        this._roomRepository = roomRepository;
                        this._practitionerRepository = practitionerRepository;
                        this._customerRepository = customerRepository;
                        this._treatmentRepository = treatmentRepository;
                }

                public async Task<IEnumerable<CalendarEventViewModel>> GetCalendarEventsAsync(CalendarBookingFilter filter)
                {
                        IEnumerable<CalendarBookingResultDto> rawData = await this._getCalendarBookingsUseCase.ExecuteAsync(filter: filter);
                        return rawData.Select(selector: dto => this.MapToViewModel(dto: dto));
                }

                public async Task<CalendarFilterLookupsDto> GetFilterLookupsAsync()
                {
                        var clinics = await this._clinicRepository.GetAllAsync();
                        var rooms = await this._roomRepository.GetAllAsync();
                        var practitioners = await this._practitionerRepository.GetAllAsync();
                        var customers = await this._customerRepository.GetAllAsync();
                        var treatments = await this._treatmentRepository.GetAllAsync();

                        return new CalendarFilterLookupsDto(
                            Clinics: clinics.Select(selector: c => new ClinicDto { Id = c.Id.Value, Name = c.Name }),
                            Rooms: rooms.Select(selector: r => new RoomDto { Id = r.Id.Value, Name = r.Name }),
                            Practitioners: practitioners.Select(selector: p => new PractitionerLookupDto { Id = p.Id.Value, DisplayName = p.Alias }),
                            Customers: customers.Select(selector: c => new CustomerSummaryDto
                            {
                                    Id = c.Id.Value,
                                    LegalFirstName = c.Details?.LegalFirstName ?? string.Empty,
                                    LegalLastName = c.Details?.LegalLastName ?? string.Empty,
                                    PhoneNumber = c.Details?.PhoneNumber?.Value ?? string.Empty,
                                    Email = c.Details?.Email?.Value ?? string.Empty
                            }),
                            Treatments: treatments.Select(selector: t => new TreatmentLookupDto(
                                Id: t.Id.Value,
                                Name: t.Name,
                                Duration: t.Duration.Value
                            ))
                        );
                }

                public async Task<IEnumerable<CalendarEventViewModel>> RefreshCalendarBookingsAsync(
                    DateTime viewStartDate,
                    DateTime viewEndDate,
                    List<Guid> clinicIds,
                    List<Guid> roomIds,
                    List<Guid> practitionerIds,
                    List<Guid> customerIds)
                {
                        CalendarBookingFilter filter = new CalendarBookingFilter(
                            ViewStartDate: viewStartDate,
                            ViewEndDate: viewEndDate,
                            ClinicIds: clinicIds.Select(selector: id => new Domain.Value_Objects.Ids.ClinicId(Value: id)),
                            RoomIds: roomIds.Select(selector: id => new Domain.Value_Objects.Ids.RoomId(Value: id)),
                            PractitionerIds: practitionerIds.Select(selector: id => new Domain.Value_Objects.Ids.PractitionerId(Value: id)),
                            CustomerIds: customerIds.Select(selector: id => new Domain.Value_Objects.Ids.CustomerId(Value: id))
                        );

                        return await this.GetCalendarEventsAsync(filter: filter);
                }

                private CalendarEventViewModel MapToViewModel(CalendarBookingResultDto dto)
                {
                        string eventColor = this.GetStatusColor(isPaid: dto.IsPaid);

                        return new CalendarEventViewModel(
                            Id: dto.BookingId.Value,
                            Title: $"{dto.TreatmentName} - {dto.CustomerName}",
                            Start: dto.StartTime,
                            End: dto.EndTime,
                            BackgroundColor: eventColor
                        );
                }

                private string GetStatusColor(bool isPaid)
                {
                        if (isPaid)
                        {
                                return COLOR_PAID_SUCCESS;
                        }

                        return COLOR_UNPAID_PENDING;
                }
        }
}
