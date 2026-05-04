using Domain.Enums;
using Domain.Value_Objects;
using Domain.Value_Objects.Ids;

namespace Domain.Entities
{
        /// <summary>
        /// Clinic where treaments are performened
        /// </summary>
        public class Clinic
        {
                /// <summary>
                /// Unique clinic identifier.
                /// </summary>
                public ClinicId Id { get; init; }

                /// <summary>
                /// Descriptive facility name.
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// Geographic location.
                /// </summary>
                public Address Address { get; private set; }

                /// <summary>
                /// Primary telephone contact.
                /// </summary>
                public PhoneNumber PhoneNumber { get; private set; }

                /// <summary>
                /// Primary email contact.
                /// </summary>
                public EmailAddress Email { get; private set; }

                private readonly List<RoomId> _roomIds = new List<RoomId>();

                /// <summary>
                /// Collection of unique identifiers for treatment rooms available at this location.
                /// The count of these rooms determines the maximum concurrent bookings.
                /// </summary>
                public IReadOnlyCollection<RoomId> RoomIds => this._roomIds.AsReadOnly();

                private readonly List<OpeningHour> _openingHours = new List<OpeningHour>();
                /// <summary>
                /// The recurring weekly schedule for this clinic.
                /// </summary>
                public IReadOnlyCollection<OpeningHour> OpeningHours => this._openingHours.AsReadOnly();

                public Clinic(
                        ClinicId id,
                        string name,
                        Address address,
                        PhoneNumber phoneNumber,
                        EmailAddress email)
                {
                        ArgumentNullException.ThrowIfNull(argument: name, paramName: nameof(name));

                        if (string.IsNullOrWhiteSpace(value: name))
                                throw new ArgumentException(message: "Name cannot be empty or whitespace.", paramName: nameof(name));


                        ArgumentNullException.ThrowIfNull(argument: id, paramName: nameof(id));
                        ArgumentNullException.ThrowIfNull(argument: address, paramName: nameof(address));
                        ArgumentNullException.ThrowIfNull(argument: phoneNumber, paramName: nameof(phoneNumber));
                        ArgumentNullException.ThrowIfNull(argument: email, paramName: nameof(email));

                        this.Id = id;
                        this.Name = name;
                        this.Address = address;
                        this.PhoneNumber = phoneNumber;
                        this.Email = email;
                }

                /// <summary>
                /// Adds a room to the clinic's capacity.
                /// </summary>
                /// <param name="roomId">The unique identifier of the room to add.</param>
                public void AddRoom(RoomId roomId)
                {
                        if (roomId is null) throw new ArgumentNullException(paramName: nameof(roomId));

                        if (this._roomIds.Contains(value: roomId))
                                throw new InvalidOperationException(message: $"Room with {roomId.Value} already exists in this clinic.");


                        this._roomIds.Add(item: roomId);
                }

                /// <summary>
                /// Removes a treatment room from the clinic, reducing its total capacity.
                /// </summary>
                /// <param name="roomId">The unique identifier of the room to remove.</param>
                public void RemoveRoom(RoomId roomId)
                {
                        if (roomId is null) throw new ArgumentNullException(paramName: nameof(roomId));

                        if (!this._roomIds.Contains(value: roomId))
                                throw new InvalidOperationException(message: $"Room with ID '{roomId.Value}' was not found in this clinic.");

                        this._roomIds.Remove(item: roomId);
                }

                /// <summary>
                /// Sets or updates the recurring schedule for a specific weekday.
                /// </summary>
                public void SetOpeningHour(OpeningHour openingHour)
                {
                        if (openingHour is null) throw new ArgumentNullException(paramName: nameof(openingHour));

                        OpeningHour? existingOpeningHour = this._openingHours.FirstOrDefault(predicate: oh => oh.Day == openingHour.Day);

                        if (existingOpeningHour is not null) this._openingHours.Remove(item: existingOpeningHour);

                        this._openingHours.Add(item: openingHour);
                }

                /// <summary>
                /// Removes the operating window for a specific day, effectively closing the clinic on that day.
                /// </summary>
                /// <param name="day">The weekday to remove from the schedule.</param>
                public void RemoveOpeningHour(Weekday day)
                {
                        OpeningHour? existing = this._openingHours.FirstOrDefault(predicate: oh => oh.Day == day);

                        if (existing is null)
                                throw new InvalidOperationException(message: $"No opening hours are currently set for {day}.");

                        this._openingHours.Remove(item: existing);
                }
        }
}
