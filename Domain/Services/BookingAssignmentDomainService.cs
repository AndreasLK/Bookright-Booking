using Domain.Entities;
using Domain.Entities.Persons;
using Domain.Value_Objects.Ids;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services
{
        /// <summary>
        /// Domain service responsible for calculating the optimal practitioner and room 
        /// for a booking based on complex domain rules and preferences.
        /// </summary>
        public class BookingAssignmentDomainService
        {
                /// <summary>
                /// Selects the most optimal room from a list of available rooms based on the treatment category.
                /// Priority 1: Rooms primarily used for this category.
                /// Priority 2: Rooms also usable for this category.
                /// </summary>
                /// <param name="availableRooms">The collection of rooms available during the requested timeslot.</param>
                /// <param name="categoryId">The category of the treatment to be performed.</param>
                /// <returns>The optimal room, or null if no suitable room is found.</returns>
                public Room? SelectBestRoom(IEnumerable<Room> availableRooms, TreatmentCategoryId categoryId)
                {
                        if (availableRooms is null || !availableRooms.Any())
                        {
                                return null;
                        }

                        if (categoryId is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(categoryId));
                        }

                        // Priority 1: Find a room that is primarily used for this exact treatment category
                        Room? primaryRoom = availableRooms.FirstOrDefault(predicate: room => room.PrimarilyUsedForId == categoryId);

                        if (primaryRoom is not null)
                        {
                                return primaryRoom;
                        }

                        // Priority 2: Find a room that is marked as 'also usable' for this category
                        Room? secondaryRoom = availableRooms.FirstOrDefault(
                            predicate: room => room.AlsoUsableFor.Contains(item: categoryId)
                        );

                        return secondaryRoom;
                }

                /// <summary>
                /// Selects the most optimal practitioner based on the customer's explicitly stated preferences and restrictions.
                /// </summary>
                /// <param name="availablePractitioners">The collection of practitioners available during the timeslot.</param>
                /// <param name="customer">The customer receiving the treatment.</param>
                /// <returns>The optimal practitioner, or null if no suitable practitioner is found.</returns>
                public Practitioner? SelectBestPractitioner(IEnumerable<Practitioner> availablePractitioners, Customer customer)
                {
                        if (availablePractitioners is null || !availablePractitioners.Any())
                        {
                                return null;
                        }

                        if (customer is null)
                        {
                                throw new ArgumentNullException(paramName: nameof(customer));
                        }

                        // Rule 1: Immediately filter out any practitioner whose gender is explicitly unwanted by the customer
                        List<Practitioner> validPractitioners = availablePractitioners
                            .Where(predicate: p => !customer.UnwantedGenders.Contains(item: p.Gender))
                            .ToList();

                        if (!validPractitioners.Any())
                        {
                                return null;
                        }

                        // Rule 2: Check for a specific preferred practitioner ID
                        if (customer.PreferredPratitionerId.HasValue)
                        {
                                Practitioner? exactMatch = validPractitioners.FirstOrDefault(
                                    predicate: p => p.Id.Value == customer.PreferredPratitionerId.Value
                                );

                                if (exactMatch is not null)
                                {
                                        return exactMatch;
                                }
                        }

                        // Rule 3: Check for a preferred gender
                        if (customer.PreferredGender.HasValue)
                        {
                                Practitioner? genderMatch = validPractitioners.FirstOrDefault(
                                    predicate: p => p.Gender == customer.PreferredGender.Value
                                );

                                if (genderMatch is not null)
                                {
                                        return genderMatch;
                                }
                        }

                        // Fallback: Return any available practitioner that survived the unwanted gender filter
                        return validPractitioners.FirstOrDefault();
                }
        }
}
