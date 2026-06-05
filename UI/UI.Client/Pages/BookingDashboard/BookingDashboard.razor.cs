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
        /// Main dashboard for viewing and orchestrating bookings across multiple clinics and rooms.
        /// </summary>
        public partial class BookingDashboard : ComponentBase
        {
                public const int DEFAULT_START_HOUR = 8;
                public const int DEFAULT_END_HOUR = 18;
                public const int DEFAULT_BOOKING_DURATION_MINUTES = 30;
                public const int DAYS_TO_VIEW_IN_WEEK = 5;
                public const int WEEK_DAY_OFFSET = 7;
                public const string DEFAULT_BOOKING_COLOR = "#198754";
                public const string UNKNOWN_CUSTOMER_NAME = "Ukendt Kunde";
                public const string UNKNOWN_TREATMENT_NAME = "Ukendt Behandling";
                public const string COMPOSITE_ID_SEPARATOR = "_";
                public const string DATE_FORMAT_PATTERN = "yyyyMMdd";
                public const string ROOM_SUBTITLE = "Behandlerlokale";

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

                public DateTime CurrentWeekStart { get; private set; }

                private QuickSearchPopUp<Practitioner>? PractitionerSearchPopUp { get; set; }
                private QuickSearchPopUp<Customer>? CustomerSearchPopUp { get; set; }

                private DateTime? _pendingBookingTime;
                private string? _pendingBookingRoomId;
                private Customer? _pendingCustomer;

                /// <summary>
                /// Initializes component state and fetches available clinics.
                /// </summary>
                protected override async Task OnInitializedAsync()
                {
                        this.CurrentWeekStart = this.GetStartOfWeek(date: DateTime.Today);

                        var clinics = await this.ClinicRepository.GetAllAsync();
                        this.AvailableClinics = clinics.ToList();
                }

                /// <summary>
                /// Calculates start date of week for given date.
                /// </summary>
                /// <param name="date">Date to evaluate.</param>
                /// <returns>Date representing Monday of week.</returns>
                private DateTime GetStartOfWeek(DateTime date)
                {
                        var difference = (WEEK_DAY_OFFSET + (date.DayOfWeek - DayOfWeek.Monday)) % WEEK_DAY_OFFSET;
                        return date.AddDays(value: -1 * difference).Date;
                }

                /// <summary>
                /// Toggles clinic selection and reloads dependent data.
                /// </summary>
                /// <param name="clinicId">Identifier for clinic.</param>
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

                /// <summary>
                /// Toggles room selection and reloads bookings.
                /// </summary>
                /// <param name="roomId">Identifier for room.</param>
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

                /// <summary>
                /// Reloads available rooms based on selected clinics.
                /// </summary>
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

                        var validRoomIds = this.AvailableRooms.Select(selector: room => room.Id).ToHashSet();
                        this.SelectedRoomIds.IntersectWith(other: validRoomIds);
                }

                /// <summary>
                /// Reloads calendar columns and active bookings for selected rooms and week.
                /// </summary>
                private async Task ReloadBookingsAndColumnsAsync()
                {
                        if (this.SelectedRoomIds.Any() == false)
                        {
                                this.CalendarColumns.Clear();
                                this.ActiveBookings.Clear();
                                return;
                        }

                        this.CalendarColumns.Clear();
                        var activeRooms = this.AvailableRooms.Where(predicate: room => this.SelectedRoomIds.Contains(item: room.Id)).ToList();

                        this.GenerateColumnsForWeek(activeRooms: activeRooms);

                        var specification = new BookingsByRoomIdsSpecification(roomIds: this.SelectedRoomIds.ToList());
                        var bookings = await this.BookingRepository.FindAsync(specification: specification);

                        var weekEnd = this.CurrentWeekStart.AddDays(value: DAYS_TO_VIEW_IN_WEEK);

                        this.ActiveBookings = bookings.Where(predicate: booking =>
                            booking.Timeslot.StartDateTime >= this.CurrentWeekStart &&
                            booking.Timeslot.StartDateTime < weekEnd).ToList();

                        await this.PopulateDictionariesAsync();
                }

                /// <summary>
                /// Generates calendar columns for each room across selected days.
                /// </summary>
                /// <param name="activeRooms">List of currently selected rooms.</param>
                private void GenerateColumnsForWeek(List<Room> activeRooms)
                {
                        for (int i = 0; i < DAYS_TO_VIEW_IN_WEEK; i++)
                        {
                                var currentDate = this.CurrentWeekStart.AddDays(value: i);

                                foreach (var room in activeRooms)
                                {
                                        var compositeId = $"{room.Id.Value}{COMPOSITE_ID_SEPARATOR}{currentDate.ToString(format: DATE_FORMAT_PATTERN)}";

                                        this.CalendarColumns.Add(item: new CalendarColumn(
                                                ColumnId: compositeId,
                                                Title: $"{currentDate:ddd dd/MM} - {room.Name}",
                                                Subtitle: ROOM_SUBTITLE,
                                                Date: currentDate
                                        ));
                                }
                        }
                }

                /// <summary>
                /// Populates lookup dictionaries for customers and treatments to prevent redundant fetching.
                /// </summary>
                private async Task PopulateDictionariesAsync()
                {
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

                /// <summary>
                /// Initiates booking flow when empty timeslot is selected.
                /// </summary>
                /// <param name="slotData">Tuple containing selected time and column identifier.</param>
                private async Task HandleEmptySlotClickedAsync((DateTime Time, string ColumnId) slotData)
                {
                        this._pendingBookingTime = slotData.Time;
                        this._pendingBookingRoomId = slotData.ColumnId;

                        this.CustomerSearchPopUp?.Open();
                        await Task.CompletedTask;
                }

                /// <summary>
                /// Searches for practitioners matching search term.
                /// </summary>
                /// <param name="searchTerm">String used for searching.</param>
                /// <returns>Collection of matched practitioners.</returns>
                private async Task<IEnumerable<Practitioner>> SearchPractitionersAsync(string searchTerm)
                {
                        var specification = new PractitionerSearchSpecification(searchTerm: searchTerm);
                        return await this.PractitionerRepository.FindAsync(specification: specification);
                }

                /// <summary>
                /// Searches for customers matching search term.
                /// </summary>
                /// <param name="searchTerm">String used for searching.</param>
                /// <returns>Collection of matched customers.</returns>
                private async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
                {
                        var specification = new CustomerSearchSpecification(searchTerm: searchTerm);
                        return await this.CustomerRepository.FindAsync(specification: specification);
                }

                /// <summary>
                /// Handles selection of customer and transitions to practitioner selection.
                /// </summary>
                /// <param name="customer">Selected customer entity.</param>
                private async Task HandleCustomerSelectedAsync(Customer customer)
                {
                        if (this._pendingBookingTime.HasValue == false || string.IsNullOrWhiteSpace(value: this._pendingBookingRoomId))
                        {
                                return;
                        }

                        this._pendingCustomer = customer;
                        this.PractitionerSearchPopUp?.Open();

                        await Task.CompletedTask;
                }

                /// <summary>
                /// Handles selection of practitioner, creates domain booking entity, and saves to repository.
                /// </summary>
                /// <param name="practitioner">Selected practitioner entity.</param>
                private async Task HandlePractitionerSelectedAsync(Practitioner practitioner)
                {
                        if (this._pendingBookingTime.HasValue == false || string.IsNullOrWhiteSpace(value: this._pendingBookingRoomId) || this._pendingCustomer == null)
                        {
                                return;
                        }

                        var idParts = this._pendingBookingRoomId.Split(separator: COMPOSITE_ID_SEPARATOR);
                        var roomId = new RoomId(Value: Guid.Parse(input: idParts[0]));

                        var clinic = this.AvailableClinics.FirstOrDefault(predicate: c => c.RoomIds.Contains(value: roomId));
                        var room = this.AvailableRooms.FirstOrDefault(predicate: r => r.Id == roomId);

                        if (room == null || clinic == null)
                        {
                                return;
                        }

                        var endTime = this._pendingBookingTime.Value.Add(value: this.DefaultBookingDuration);
                        var timeslot = new TimeSlot(startDateTime: this._pendingBookingTime.Value, endDateTime: endTime);

                        var newBooking = new Booking(
                                id: new BookingId(Value: Guid.NewGuid()),
                                customer: this._pendingCustomer.Id,
                                practitioner: practitioner.Id,
                                room: roomId,
                                treatment: new TreatmentId(Value: Guid.Empty),
                                clinic: clinic.Id,
                                timeslot: timeslot
                        );

                        await this.BookingRepository.AddAsync(entity: newBooking);

                        this.ClearPendingBookingState();
                        await this.ReloadBookingsAndColumnsAsync();
                }

                /// <summary>
                /// Resets pending internal state used during booking creation.
                /// </summary>
                private void ClearPendingBookingState()
                {
                        this._pendingBookingTime = null;
                        this._pendingBookingRoomId = null;
                        this._pendingCustomer = null;
                }

                /// <summary>
                /// Retrieves formatted name of customer from dictionary.
                /// </summary>
                /// <param name="customerId">Identifier for customer.</param>
                /// <returns>Formatted full name or fallback string.</returns>
                private string GetCustomerName(CustomerId customerId)
                {
                        if (this._customerDictionary.TryGetValue(key: customerId, value: out var customer))
                        {
                                return $"{customer.PreferredFirstName ?? customer.LegalFirstName} {customer.PreferredLastName ?? customer.LegalLastName}";
                        }
                        return UNKNOWN_CUSTOMER_NAME;
                }

                /// <summary>
                /// Retrieves treatment name from dictionary.
                /// </summary>
                /// <param name="treatmentId">Identifier for treatment.</param>
                /// <returns>Name of treatment or fallback string.</returns>
                private string GetTreatmentName(TreatmentId treatmentId)
                {
                        if (this._treatmentDictionary.TryGetValue(key: treatmentId, value: out var treatment))
                        {
                                return treatment.Name;
                        }
                        return UNKNOWN_TREATMENT_NAME;
                }

                /// <summary>
                /// Extracts start time from booking entity.
                /// </summary>
                /// <param name="booking">Target booking entity.</param>
                /// <returns>Start date and time.</returns>
                private DateTime GetBookingStart(Booking booking)
                {
                        return booking.Timeslot.StartDateTime;
                }

                /// <summary>
                /// Extracts end time from booking entity.
                /// </summary>
                /// <param name="booking">Target booking entity.</param>
                /// <returns>End date and time.</returns>
                private DateTime GetBookingEnd(Booking booking)
                {
                        return booking.Timeslot.EndDateTime;
                }

                /// <summary>
                /// Determines CSS color string for booking rendering.
                /// </summary>
                /// <param name="booking">Target booking entity.</param>
                /// <returns>Hexadecimal color string.</returns>
                private string GetBookingColor(Booking booking)
                {
                        return DEFAULT_BOOKING_COLOR;
                }

                /// <summary>
                /// Maps booking back to composite column identifier for UI placement.
                /// </summary>
                /// <param name="booking">Target booking entity.</param>
                /// <returns>Composite string matching calendar column identifier.</returns>
                private string GetBookingColumnId(Booking booking)
                {
                        var formattedDate = booking.Timeslot.StartDateTime.ToString(format: DATE_FORMAT_PATTERN);
                        return $"{booking.RoomId.Value}{COMPOSITE_ID_SEPARATOR}{formattedDate}";
                }
        }
}
