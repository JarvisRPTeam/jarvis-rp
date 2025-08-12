require('./ui/admin_panel/admin_panel.js');

let lastPosition = null;
let accumulatedDistance = 0;

// Track distance every render tick
mp.events.add('render', () => {
    const player = mp.players.local;

    if (player.isInAnyVehicle(false)) {
        const vehicle = player.vehicle;

        // Player must be driver
        if (vehicle.getPedInSeat(-1) === player.handle) {
            const currentPosition = vehicle.position;

            if (lastPosition) {
                const distance = mp.game.gameplay.getDistanceBetweenCoords(
                    lastPosition.x, lastPosition.y, lastPosition.z,
                    currentPosition.x, currentPosition.y, currentPosition.z,
                    true
                );

                // Ignore tiny movements & teleport spikes
                if (distance > 0.05 && distance < 100.0) {
                    accumulatedDistance += distance;
                }
            }

            // Save copy of position for next frame
            lastPosition = {
                x: currentPosition.x,
                y: currentPosition.y,
                z: currentPosition.z
            };
        } else {
            lastPosition = null;
        }
    } else {
        lastPosition = null;
    }
});

// Every 10 seconds send accumulated meters to server
setInterval(() => {
    if (accumulatedDistance > 0) {
        // Send meters (server converts to km)
        mp.events.callRemote('Vehicle_AddDistance', accumulatedDistance);

        // Debug in chat
        mp.gui.chat.push(
            `~y~[Mileage]~w~ Sent ${(accumulatedDistance / 1000).toFixed(3)} km to server`
        );

        // Reset
        accumulatedDistance = 0;
    }
}, 10000);

// Server tells client to update mileage display
mp.events.add('Vehicle_UpdateMileageDisplay', (newMileage) => {
    mp.gui.chat.push(`~g~[Mileage]~w~ Updated: ${newMileage.toFixed(2)} km`);
});
