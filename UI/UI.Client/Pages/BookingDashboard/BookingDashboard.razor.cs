using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Interfaces.Repositories;
using Domain.Specifications.Bookings;
using Domain.Specifications.Customers;
using Domain.Specifications.Practitioners;
using Domain.Specifications.Rooms;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;
using Microsoft.AspNetCore.Components;
using UI.Client.Components.Calendar;
using UI.Client.Components.PopUps;

namespace UI.Client.Pages.BookingDashboard
{
        /// <summary>
        /// The main dashboard for viewing and orchestrating bookings across multiple clinics and rooms.
        /// </summary>
        public partial class BookingDashboard : ComponentBase
        {
                public const int DEFAULT_START_HOUR = 8;
                public const int DEFAULT_END_HOUR = 18;
                public const int DEFAULT_BOOKING_DURATION_MINUTES = 30;

                [Inject] private IClinicRepository ClinicRepository { get; set; } = default!;
                [Inject] private IRoomRepository RoomRepository { get; set; } = default!;
                [Inject] private IBookingRepository BookingRepository { get; set; } = default!;
                [Inject] private IPractitionerRepository PractitionerRepository { get; set; } = default!;
                [Inject] private ICustomerRepository CustomerRepository { get; set; } = default!;
                [Inject] private ITreatmentRepository TreatmentRepository { get; set; } = default!;

                public List<Clinic> AvailableClinics { get; private set; } = new();
                public List<Room> AvailableRooms { get; private set; } = new();
                public List<Booking> ActiveBookings { get; private set; } = new();
                public List<CalendarColumn> CalendarColumns { get; private set; } = new();

                public HashSet<ClinicId> SelectedClinicIds { get; private set; } = new();
                public HashSet<RoomId> SelectedRoomIds { get; private set; } = new();

                private Dictionary<CustomerId, Customer> _customerDictionary = new();
                private Dictionary<TreatmentId, Treatment> _treatmentDictionary = new();

                public TimeSpan DefaultBookingDuration { get; private set; } = TimeSpan.FromMinutes(value: DEFAULT_BOOKING_DURATION_MINUTES);

                private QuickSearchPopUp<Practitioner>? PractitionerSearchPopUp { get; set; }
                private QuickSearchPopUp<Customer>? CustomerSearchPopUp { get; set; }

                private DateTime? _pendingBookingTime;
                private string? _pendingBookingRoomId;

                protected override async Task OnInitializedAsync()
                {
                        var clinics = await this.ClinicRepository.GetAllAsync();
                        this.AvailableClinics = clinics.ToList();
                }

                public async Task ToggleClinicAsync(ClinicId clinicId)
                {
                        if (this.SelectedClinicIds.Contains(item: clinicId))
                        {
                                this.SelectedClinicIds.Remove(item: clinicId);
                        }
                        else
                        {
                                this.SelectedClinicIds.Add(item: clinicId);
                        }

                        await this.ReloadRoomsAsync();
                        await this.ReloadBookingsAndColumnsAsync();
                }

                public async Task ToggleRoomAsync(RoomId roomId)
                {
                        if (this.SelectedRoomIds.Contains(item: roomId))
                        {
                                this.SelectedRoomIds.Remove(item: roomId);
                        }
                        else
                        {
                                this.SelectedRoomIds.Add(item: roomId);
                        }

                        await this.ReloadBookingsAndColumnsAsync();
                }

                private async Task ReloadRoomsAsync()
                {
                        if (this.SelectedClinicIds.Any() == false)
                        {
                                this.AvailableRooms.Clear();
                                this.SelectedRoomIds.Clear();
                                return;
                        }

                        var activeClinics = this.AvailableClinics
                            .Where(predicate: clinic => this.SelectedClinicIds.Contains(item: clinic.Id))
                            .ToList();

                        var targetedRoomIds = activeClinics
                            .SelectMany(selector: clinic => clinic.RoomIds)
                            .ToList();

                        if (targetedRoomIds.Any() == false)
                        {
                                this.AvailableRooms.Clear();
                                this.SelectedRoomIds.Clear();
                                return;
                        }

                        var specification = new RoomsByIdsSpecification(roomIds: targetedRoomIds);
                        var rooms = await this.RoomRepository.FindAsync(specification: specification);

                        this.AvailableRooms = rooms.ToList();

                        var validRoomIds = this.AvailableRooms.Select(selector: r => r.Id).ToHashSet();
                        this.SelectedRoomIds.IntersectWith(other: validRoomIds);
                }

                private async Task ReloadBookingsAndColumnsAsync()
                {
                        if (this.SelectedRoomIds.Any() == false)
                        {
                                this.CalendarColumns.Clear();
                                this.ActiveBookings.Clear();
                                return;
                        }

                        this.CalendarColumns = this.AvailableRooms
                            .Where(predicate: room => this.SelectedRoomIds.Contains(item: room.Id))
                            .Select(selector: room => new CalendarColumn(
                                ColumnId: room.Id.Value.ToString(),
                                Title: room.Name,
                                Subtitle: "Behandlerlokale",
                                Date: DateTime.Today))
                            .ToList();

                        var specification = new BookingsByRoomIdsSpecification(roomIds: this.SelectedRoomIds.ToList());
                        var bookings = await this.BookingRepository.FindAsync(specification: specification);

                        this.ActiveBookings = bookings.ToList();

                        foreach (var booking in this.ActiveBookings)
                        {
                                if (this._customerDictionary.ContainsKey(key: booking.CustomerId) == false)
                                {
                                        var customer = await this.CustomerRepository.GetByIdAsync(id: booking.CustomerId.Value);
                                        if (customer != null)
                                        {
                                                this._customerDictionary.Add(key: booking.CustomerId, value: customer);
                                        }
                                }

                                if (this._treatmentDictionary.ContainsKey(key: booking.TreatmentId) == false)
                                {
                                        var treatment = await this.TreatmentRepository.GetByIdAsync(id: booking.TreatmentId.Value);
                                        if (treatment != null)
                                        {
                                                this._treatmentDictionary.Add(key: booking.TreatmentId, value: treatment);
                                        }
                                }
                        }
                }

                private async Task HandleEmptySlotClickedAsync((DateTime Time, string ColumnId) slotData)
                {
                        this._pendingBookingTime = slotData.Time;
                        this._pendingBookingRoomId = slotData.ColumnId;

                        this.CustomerSearchPopUp?.Open();
                        await Task.CompletedTask;
                }

                private async Task<IEnumerable<Practitioner>> SearchPractitionersAsync(string searchTerm)
                {
                        var specification = new PractitionerSearchSpecification(searchTerm: searchTerm);
                        return await this.PractitionerRepository.FindAsync(specification: specification);
                }

                private async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
                {
                        var specification = new CustomerSearchSpecification(searchTerm: searchTerm);
                        return await this.CustomerRepository.FindAsync(specification: specification);
                }

                private async Task HandleCustomerSelectedAsync(Customer customer)
                {
                        if (this._pendingBookingTime.HasValue == false || string.IsNullOrWhiteSpace(value: this._pendingBookingRoomId))
                        {
                                return;
                        }

                        this.PractitionerSearchPopUp?.Open();
                        await Task.CompletedTask;
                }

                private async Task HandlePractitionerSelectedAsync(Practitioner practitioner)
                {

                        this._pendingBookingTime = null;
                        this._pendingBookingRoomId = null;

                        await this.ReloadBookingsAndColumnsAsync();
                }


                private string GetCustomerName(CustomerId customerId)
                {
                        if (this._customerDictionary.TryGetValue(key: customerId, value: out var customer))
                        {
                                return $"{customer.PreferredFirstName ?? customer.LegalFirstName} {customer.PreferredLastName ?? customer.LegalLastName}";
                        }
                        return "Ukendt Kunde";
                }

                private string GetTreatmentName(TreatmentId treatmentId)
                {
                        if (this._treatmentDictionary.TryGetValue(key: treatmentId, value: out var treatment))
                        {
                                return treatment.Name;
                        }
                        return "Ukendt Behandling";
                }


                private DateTime GetBookingStart(Booking booking)
                {
                        return booking.Timeslot.StartDateTime;
                }

                private DateTime GetBookingEnd(Booking booking)
                {
                        return booking.Timeslot.EndDateTime;
                }

                private string GetBookingColor(Booking booking)
                {
                        return "#198754";
                }

                private string GetBookingColumnId(Booking booking)
                {
                        return booking.RoomId.Value.ToString();
                }
        }
}
