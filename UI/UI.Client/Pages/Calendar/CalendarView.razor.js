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
                slotDuration: '00:15:00',   // Set visual grid to 15 minutes
                snapDuration: '00:15:00',   // Set snapping precision to 15 minutes
                slotLabelInterval: '01:00', // Only label every hour to keep the Y-axis clean
                allDaySlot: false,
                selectable: true,
                selectMirror: true,
                events: eventsData,

                select: function (selectionInfo) {
                        dotNetObject.invokeMethodAsync('OnTimeSlotSelected', selectionInfo.startStr, selectionInfo.endStr)
                                .catch(error => console.error("Error calling C# OnTimeSlotSelected:", error));

                        calendarInstance.unselect();
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
