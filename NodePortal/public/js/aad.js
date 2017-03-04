
// var adal = require('adal-angular');

// var ADAL = new adal.AuthenticationContext({
//     instance: 'https://login.microsoftonline.com/',
//     tenant: 'common', //COMMON OR YOUR TENANT ID

//     clientId: '64228cc7-deee-4f21-af43-699ee84f8db0', //This is your client ID
//     redirectUri: 'http://localhost:3000/chart', //This is your redirect URI

//     callback: userSignedIn,
//     popUp: true
// });

var ADAL = new AuthenticationContext({
    instance: 'https://login.microsoftonline.com/',
    tenant: 'common', //COMMON OR YOUR TENANT ID

    clientId: '64228cc7-deee-4f21-af43-699ee84f8db0', //This is your client ID
    redirectUri: 'http://localhost:3000/chart', //This is your redirect URI

    callback: userSignedIn,
    popUp: true
});

function signIn() {
    ADAL.login();
}

function signOut() {
    ADAL.logOut();
}

function userSignedIn(err, token) {
    console.log('userSignedIn called');
    if (!err) {
        console.log('token: ' + token);
        document.cookie = 'token=' + token;
        showWelcomeMessage();
    } else {
        console.error('error: ' + err);
    }
}

function showWelcomeMessage() {
    var user = ADAL.getCachedUser();
    var divWelcome = document.getElementById('WelcomeMessage');
    divWelcome.innerHTML = 'Welcome ' + JSON.stringify(ADAL);
}