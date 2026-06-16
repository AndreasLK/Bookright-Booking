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
                locale: 'da',
                firstDay: 1,
                slotMinTime: '08:00:00',
                slotMaxTime: '18:00:00',
                slotDuration: '00:15:00',   // Visual grid at 15 minutes
                snapDuration: '00:15:00',   // Snapping logic at 15 minutes
                slotLabelInterval: '01:00',
                allDaySlot: false,
                selectable: true,
                selectMirror: true,
                events: eventsData,

                // When user selects a NEW time slot by dragging or clicking empty space
                select: function (selectionInfo) {
                        dotNetObject.invokeMethodAsync('OnTimeSlotSelected', selectionInfo.startStr, selectionInfo.endStr)
                                .catch(error => console.error("Error calling C# OnTimeSlotSelected:", error));

                        calendarInstance.unselect();
                },

                // When the user clicks an EXISTING event
                eventClick: function (info) {
                        info.jsEvent.preventDefault(); // Prevents URL navigation

                        dotNetObject.invokeMethodAsync('OnBookingEventClicked', info.event.id)
                                .catch(error => console.error("Error calling C# OnBookingEventClicked:", error));
                }
        });

        calendarInstance.render();
}

export function updateCalendarEvents(containerId, newEvents) {
        if (calendarInstance) {
                calendarInstance.removeAllEvents();
                calendarInstance.addEventSource(newEvents);
        }
}

// Dynamically sets the selection block size so it perfectly previews the treatment duration
export function setTreatmentPreviewDuration(durationStr) {
        if (calendarInstance && durationStr) {
                calendarInstance.setOption('defaultTimedEventDuration', durationStr);
        }
}
