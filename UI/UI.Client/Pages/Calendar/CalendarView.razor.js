let calendarInstance = null;

export function initializeCalendar(containerId, eventsData, dotNetObject) {
        const containerElement = document.getElementById(containerId);

        if (!containerElement) {
                console.error(`Calendar container with id '${containerId}' not found.`);
                return;
        }

        // Initialize the FullCalendar instance
        calendarInstance = new FullCalendar.Calendar(containerElement, {
                initialView: 'timeGridWeek',
                locale: 'da',               // Set language to Danish
                firstDay: 1,                // Start week on Monday
                slotMinTime: '08:00:00',    // Clinic opening time
                slotMaxTime: '18:00:00',    // Clinic closing time
                allDaySlot: false,          // Hide the "All Day" section at the top
                selectable: true,           // Enables click-and-drag to create
                selectMirror: true,         // Shows a visual placeholder while dragging
                events: eventsData,         // Load the initial data

                // --- THE BRIDGE TO C# ---
                // When the user finishes dragging, this sends the start and end times
                // back to your C# method: [JSInvokable] OnTimeSlotSelected
                select: function (selectionInfo) {
                        dotNetObject.invokeMethodAsync('OnTimeSlotSelected', selectionInfo.startStr, selectionInfo.endStr)
                                .catch(error => console.error("Error calling C# OnTimeSlotSelected:", error));

                        calendarInstance.unselect(); // Clears the temporary JS visual selection
                }
        });

        calendarInstance.render();
}

// Called by C# whenever filters change or a new booking is saved
export function updateCalendarEvents(containerId, newEvents) {
        if (calendarInstance) {
                calendarInstance.removeAllEvents();
                calendarInstance.addEventSource(newEvents);
        }
}
