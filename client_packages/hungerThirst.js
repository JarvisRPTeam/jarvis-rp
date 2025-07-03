let hudBrowser = null;

mp.events.add('playerReady', () => {
  if (!hudBrowser) {
    hudBrowser = mp.browsers.new('package://ui/hud.html');
  }
});

// mp.events.add('client:updateStats', (hunger, thirst) => {
//   if (hudBrowser) {
//     hudBrowser.execute(`mp.events.call('client:updateStats', ${hunger}, ${thirst});`);
//   }
// });

let myTimer = setInterval(() => {
    mp.events.callRemote('Player:DrainThirst'); 
    mp.events.callRemote('Player:DrainHunger');
}, 300000);