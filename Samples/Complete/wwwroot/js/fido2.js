coerceToArrayBuffer = function (thing, name) {
    if (typeof thing === "string") {
        // base64url to base64
        thing = thing.replace(/-/g, "+").replace(/_/g, "/");

        // base64 to Uint8Array
        var str = window.atob(thing);
        var bytes = new Uint8Array(str.length);
        for (var i = 0; i < str.length; i++) {
            bytes[i] = str.charCodeAt(i);
        }
        thing = bytes;
    }

    // Array to Uint8Array
    if (Array.isArray(thing)) {
        thing = new Uint8Array(thing);
    }

    // Uint8Array to ArrayBuffer
    if (thing instanceof Uint8Array) {
        thing = thing.buffer;
    }

    // error if none of the above worked
    if (!(thing instanceof ArrayBuffer)) {
        throw new TypeError("could not coerce '" + name + "' to ArrayBuffer");
    }

    return thing;
};

coerceToBase64Url = function (thing) {
    // Array or ArrayBuffer to Uint8Array
    if (Array.isArray(thing)) {
        thing = Uint8Array.from(thing);
    }

    if (thing instanceof ArrayBuffer) {
        thing = new Uint8Array(thing);
    }

    // Uint8Array to base64
    if (thing instanceof Uint8Array) {
        var str = "";
        var len = thing.byteLength;

        for (var i = 0; i < len; i++) {
            str += String.fromCharCode(thing[i]);
        }
        thing = window.btoa(str);
    }

    if (typeof thing !== "string") {
        throw new Error("could not coerce to string");
    }

    // base64 to base64url
    // NOTE: "=" at the end of challenge is optional, strip it off here
    thing = thing.replace(/\+/g, "-").replace(/\//g, "_").replace(/=*$/g, "");

    return thing;
};

if (window.PublicKeyCredential === undefined ||
    typeof window.PublicKeyCredential !== "function") {
    document.getElementById("fido2NotSupportedWarning")
        .style.display = "block";
}

document.getElementById('fido2-signin')
    .addEventListener('submit', handleSignInSubmit);
async function handleSignInSubmit(event) {
    event.preventDefault();

    let username = this.username.value;
    let returnUrl = this.returnUrl.value;

    var formData = new FormData();
    formData.append('username', username);

    // send to server for registering
    let makeAssertionOptions;
    try {
        var res = await fetch('api/fido2/pwassertionOptions', {
            method: 'POST', // or 'PUT'
            body: formData, // data can be `string` or {object}!
            headers: {
                'Accept': 'application/json'
            }
        });

        makeAssertionOptions = await res.json();
    } catch (e) {
        // console.error(e);
        showErrorAlert("Request to server failed");
    }

    // console.log("Assertion Options Object", makeAssertionOptions);

    // show options error to user
    if (makeAssertionOptions.status !== "ok") {
        // console.error(makeAssertionOptions.errorMessage);
        showErrorAlert("Error in assertion options");
        return;
    }

    const challenge = makeAssertionOptions.challenge.replace(/-/g, "+").replace(/_/g, "/");
    makeAssertionOptions.challenge = Uint8Array.from(atob(challenge), c => c.charCodeAt(0));

    makeAssertionOptions.allowCredentials.forEach(function (listItem) {
        var fixedId = listItem.id.replace(/\_/g, "/").replace(/\-/g, "+");
        listItem.id = Uint8Array.from(atob(fixedId), c => c.charCodeAt(0));
    });

    // console.log("Assertion options", makeAssertionOptions);

    Swal.fire({
        title: 'Logging In...',
        text: 'Tap your security key to login.',
        imageUrl: "/images/securitykey.min.svg",
        showCancelButton: true,
        showConfirmButton: false,
        focusConfirm: false,
        focusCancel: false
    });

    // ask browser for credentials (browser will ask connected authenticators)
    let credential;
    try {
        credential = await navigator.credentials.get({ publicKey: makeAssertionOptions })
    } catch (err) {
        // console.error(err);
        showErrorAlert("Error creating assertion");
    }

    try {
        await verifyAssertionWithServer(credential, returnUrl);
    } catch (e) {
        // console.error(e);
        showErrorAlert("Could not verify assertion");
    }
}

async function verifyAssertionWithServer(assertedCredential, returnUrl) {
    // Move data into Arrays incase it is super long
    let authData = new Uint8Array(assertedCredential.response.authenticatorData);
    let clientDataJSON = new Uint8Array(assertedCredential.response.clientDataJSON);
    let rawId = new Uint8Array(assertedCredential.rawId);
    let sig = new Uint8Array(assertedCredential.response.signature);
    const data = {
        id: assertedCredential.id,
        rawId: coerceToBase64Url(rawId),
        type: assertedCredential.type,
        extensions: assertedCredential.getClientExtensionResults(),
        response: {
            authenticatorData: coerceToBase64Url(authData),
            clientDataJson: coerceToBase64Url(clientDataJSON),
            signature: coerceToBase64Url(sig)
        }
    };

    let response;
    try {
        let res = await fetch("/api/fido2/pwmakeAssertion", {
            method: 'POST', // or 'PUT'
            body: JSON.stringify(data), // data can be `string` or {object}!
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            }
        });

        response = await res.json();
    } catch (e) {
        // console.error(e);
        showErrorAlert("Request to server failed");
        throw e;
    }

    console.log("Assertion Object", response);

    // show error
    if (response.status !== "ok") {
        // console.error(response.errorMessage);
        showErrorAlert("Error doing assertion");
        return;
    }

    // show success message
    await Swal.fire({
        title: 'Signed In',
        text: 'You\'ve signed in successfully.',
        type: 'success',
        timer: 2000
    });

    if (returnUrl != null && returnUrl.length
        && (returnUrl.startsWith() || returnUrl.startsWith(window.location.origin + "/"))) {
        window.location.href = returnUrl;
    } else {
        window.location.href = "/";
    }
}

document.getElementById("fido2-register")
    .addEventListener('submit', handleRegisterSubmit);
async function handleRegisterSubmit(event) {
    event.preventDefault();

    let username = this.username.value;
    let displayName = this.displayName.value;
    let returnUrl = this.returnUrl.value;

    // possible values: none, direct, indirect
    let attestation_type = "none";
    // possible values: <empty>, platform, cross-platform
    let authenticator_attachment = "";

    // possible values: preferred, required, discouraged
    let user_verification = "preferred";

    // possible values: true,false
    let require_resident_key = false;

    // prepare form post data
    var data = new FormData();
    data.append('username', username);
    data.append('displayName', displayName);
    data.append('attType', attestation_type);
    data.append('authType', authenticator_attachment);
    data.append('userVerification', user_verification);
    data.append('requireResidentKey', require_resident_key);

    // send to server for registering
    let makeCredentialOptions;
    try {
        makeCredentialOptions = await fetchMakeCredentialOptions(data);

    } catch (e) {
        console.error(e);
        let msg = "Something went really wrong";
        showErrorAlert(msg);
    }

    // console.log("Credential Options Object", makeCredentialOptions);

    if (makeCredentialOptions.status !== "ok") {
        // console.log(makeCredentialOptions.errorMessage);
        showErrorAlert("Error creating credential options");
        return;
    }

    // Turn the challenge back into the accepted format of padded base64
    makeCredentialOptions.challenge = coerceToArrayBuffer(makeCredentialOptions.challenge);
    // Turn ID into a UInt8Array Buffer for some reason
    makeCredentialOptions.user.id = coerceToArrayBuffer(makeCredentialOptions.user.id);

    makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map((c) => {
        c.id = coerceToArrayBuffer(c.id);
        return c;
    });

    if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null) {
        makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;
    }

    // console.log("Credential Options Formatted", makeCredentialOptions);

    Swal.fire({
        title: 'Registering...',
        text: 'Tap your security key to finish registration.',
        imageUrl: "/images/securitykey.min.svg",
        showCancelButton: true,
        showConfirmButton: false,
        focusConfirm: false,
        focusCancel: false
    });

    // console.log("Creating PublicKeyCredential...");
    // console.log(makeCredentialOptions);
    let newCredential;
    try {
        newCredential = await navigator.credentials.create({
            publicKey: makeCredentialOptions
        });
    } catch (e) {
        var msg = "Could not create credentials in browser. The username may already be registered with your authenticator. Try changing username or authenticator.";
        // console.error(msg, e);
        showErrorAlert(msg);
    }

    // console.log("PublicKeyCredential Created", newCredential);

    try {
        registerNewCredential(newCredential, returnUrl);
    } catch (e) {
        // console.error("Error creating credential", e);
        showErrorAlert("Error creating credential");
    }
}

async function fetchMakeCredentialOptions(formData) {
    let response = await fetch('api/fido2/pwmakeCredentialOptions', {
        method: 'POST', // or 'PUT'
        body: formData, // data can be `string` or {object}!
        headers: {
            'Accept': 'application/json'
        }
    });

    let data = await response.json();

    return data;
}

async function registerNewCredential(newCredential, returnUrl) {
    // Move data into Arrays incase it is super long
    let attestationObject = new Uint8Array(newCredential.response.attestationObject);
    let clientDataJSON = new Uint8Array(newCredential.response.clientDataJSON);
    let rawId = new Uint8Array(newCredential.rawId);

    const data = {
        id: newCredential.id,
        rawId: coerceToBase64Url(rawId),
        type: newCredential.type,
        extensions: newCredential.getClientExtensionResults(),
        response: {
            AttestationObject: coerceToBase64Url(attestationObject),
            clientDataJson: coerceToBase64Url(clientDataJSON)
        }
    };

    let response;
    try {
        response = await registerCredentialWithServer(data);
    } catch (e) {
        console.error(e);
        showErrorAlert("Error creating credential");
    }

    // console.log("Credential Object", response);

    // show error
    if (response.status !== "ok") {
        let msg = "Error creating credential";
        console.error(msg);
        console.error(response.errorMessage);
        showErrorAlert(msg);
        return;
    }

    // show success 
    Swal.fire({
        title: 'Registration Successful!',
        text: 'You\'ve registered successfully.',
        type: 'success',
        timer: 2000
    });

    if (returnUrl != null && returnUrl.length
        && (returnUrl.startsWith() || returnUrl.startsWith(window.location.origin + "/"))) {
        window.location.href = returnUrl;
    } else {
        window.location.href = "/";
    }
}

async function registerCredentialWithServer(formData) {
    let response = await fetch('api/fido2/pwmakeCredential', {
        method: 'POST', // or 'PUT'
        body: JSON.stringify(formData), // data can be `string` or {object}!
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        }
    });

    let data = await response.json();

    return data;
}

function showErrorAlert(msg) {
    Swal.fire({
        title: 'Error',
        text: msg,
        icon: 'error'
    });
}
