mp.keys.bind(0x45, true, function() { // E key
    if (mp.players.local.isInAnyVehicle(false)) {
        mp.events.callRemote("toggleEngine");
    }
});

// L key to lock/unlock vehicle
mp.keys.bind(0x4C, true, function () { // 0x4C = L
    mp.events.callRemote("toggleLock");
});



//Vehicle distance tracking
let lastPosition = null;
let accumulatedDistance = 0;

mp.events.add('render', () => {
    const player = mp.players.local;

    if (player.isInAnyVehicle(false)) {
        const vehicle = player.vehicle;

        // Only if driving
        if (vehicle.getPedInSeat(-1) === player.handle) {
            const currentPosition = vehicle.position;

            if (lastPosition) {
                const distance = mp.game.gameplay.getDistanceBetweenCoords(
                    lastPosition.x, lastPosition.y, lastPosition.z,
                    currentPosition.x, currentPosition.y, currentPosition.z,
                    true
                );

                if (distance > 0.05 && distance < 100.0) {
                    accumulatedDistance += distance;
                }
            }

            lastPosition = currentPosition;
        }
    } else {
        lastPosition = null;
    }
});

// Send every 10 seconds to server
setInterval(() => {
    if (accumulatedDistance > 0) {
        mp.events.callRemote('Vehicle_AddDistance', accumulatedDistance);
        accumulatedDistance = 0;
    }
}, 10000);



//SPEEED
// mp.events.add('render', () => {
//     const player = mp.players.local;

//     if (player.vehicle) {
//         let speedMps = player.vehicle.getSpeed();
//         let speedKmh = Math.round(speedMps * 3.6);

//         mp.game.graphics.drawText(`Speed: ${speedKmh} km/h`, [0.5, 0.9], {
//             font: 4,
//             color: [255, 255, 255, 255],
//             scale: [0.5, 0.5],
//             outline: true
//         });

//         // Send to server every 5 seconds
//         if(speedKmh != 0)
//         {
//             if (!player.lastSpeedSendTime || Date.now() - player.lastSpeedSendTime > 5000) {
//                 player.lastSpeedSendTime = Date.now();
//                 mp.gui.chat.push(`Sending speed: ${speedKmh}`);
//                 mp.events.callRemote('sendSpeedToServer', speedKmh);
//             }
//         } 
        
//     }
// });

