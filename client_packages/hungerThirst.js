// Client-side script (client_packages or similar)
let hunger = 100;
let thirst = 100;

function updateStatsUI(h, t) {
    hunger = h;
    thirst = t;
    // Update your UI bars here
}

// Receive server stats update
mp.events.add('Player:UpdateStats', (h, t) => {
    updateStatsUI(h, t);
});

// Send update event every 60 seconds
setInterval(() => {
    mp.events.callRemote('Player:UpdateStats');
}, 60000);
