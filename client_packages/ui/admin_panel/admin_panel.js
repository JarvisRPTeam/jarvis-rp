let adminPanel = null;
let panelVisible = false;
let playersBrowser = null;
let vehicleBrowser = null;

mp.keys.bind(0xC0, false, function() {

    if (adminPanel !== null || playersBrowser !== null || vehicleBrowser !== null) {
        if (adminPanel !== null) {
            adminPanel.destroy();
            adminPanel = null;
            panelVisible = false;
        }
        if (playersBrowser !== null) {
            playersBrowser.destroy();
            playersBrowser = null;
        }
        if (vehicleBrowser !== null) {
            vehicleBrowser.destroy();
            vehicleBrowser = null;
        }
        mp.gui.cursor.show(false, false);
        return;
    }

    if (mp.gui.cursor.visible && !panelVisible) {
        return;
    }

    if (!panelVisible) {
        if (adminPanel === null) {
            adminPanel = mp.browsers.new('package://ui/admin_panel/index.html');
        }
        panelVisible = true;
        mp.gui.cursor.show(true, true);
    } else {
        if (adminPanel !== null) {
            adminPanel.destroy();
            adminPanel = null;
        }
        panelVisible = false;
        mp.gui.cursor.show(false, false);
    }

});

mp.events.add('adminPanel:showPlayers', () => {
    if (playersBrowser === null) {
        playersBrowser = mp.browsers.new('package://ui/admin_panel/players.html');
        mp.events.callRemote('adminPanel:requestPlayers');
    }
});

mp.events.add('adminPanel:showVehiclesPanel', () => {
    if (vehicleBrowser === null) {
        vehicleBrowser = mp.browsers.new('package://ui/admin_panel/vehicles.html');
        mp.events.callRemote('adminPanel:requestVehicles');
    }
});


mp.events.add('adminPanel:receivePlayers', (json) => {
    if (playersBrowser) {
        // Parse the JSON and send the panel model to the browser
        playersBrowser.execute(`showPlayersPanel(${json})`);
    }
});

mp.events.add('browserDomReady', (browser) => {
    if (playersBrowser && browser === playersBrowser) {
        playersBrowser.execute(`
            window.requestPlayers = function() {
                mp.trigger('adminPanel:requestPlayers');
            };
            document.getElementById('backBtn').onclick = function() {
                mp.trigger('adminPanel:closePlayers');
            };
        `);
    }
});

mp.events.add('adminPanel:closePlayers', () => {
    if (playersBrowser !== null) {
        playersBrowser.destroy();
        playersBrowser = null;
    }
});

mp.events.add('adminPanel:closeVehicles', () => {
    if (vehicleBrowser !== null) {
        vehicleBrowser.destroy();
        vehicleBrowser = null;
    }
});