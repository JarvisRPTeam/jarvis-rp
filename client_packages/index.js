require("./hungerThirst.js");

// client_money.js
mp.events.add('UpdateMoneyUI', (money) => {
    let moneyText = mp.browsers.exists(global.moneyBrowser) ? global.moneyBrowser : null;

    if (!moneyText) {
        global.moneyBrowser = mp.browsers.new('package://ui/money.html');
        moneyText = global.moneyBrowser;
    }

    moneyText.execute(`updateMoney(${money});`);
});
