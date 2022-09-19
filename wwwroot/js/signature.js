var pad = new SignaturePad(signatureCanvas);

async function clearPad() {
    try {
        signatureCanvas.getContext("2d").clearRect(0, 0, signatureCanvas.width, signatureCanvas.height);
        console.log("Signature pad cleared");
    } catch (err) {
        console.log("Cannot clear signature pad:" + err);
        alert("Cannot clear signature pad!")
    }
}

async function saveSignature() {
    try {
        // Get the raw image data from the data URL, but remove the metadata
        let imageData = signatureCanvas.toDataURL("image/png").replace("data:image/png;base64,", "");
        // Send the data, as a string, to the SaveSignature() handler in the code behind
        await $.ajax({
            type: "POST",
            url: "Create?handler=SaveSignature",
            data: JSON.stringify(imageData),
            contentType: "application/json",
            // Needed to allow the app to save files to the wwwroot
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (msg) {
                console.log("Signature saved: " + msg);
                alert("Signature saved!");
            },
        });
    } catch (err) {
        console.log("Cannot save signature:" + err);
        alert("Cannot save signature!")
    }
}

async function updateSignature() {
    try {
        // Get the raw image data from the data URL, but remove the metadata
        let imageData = signatureCanvas.toDataURL("image/png").replace("data:image/png;base64,", "");
        // Send the data, as a string, to the SaveSignature() handler in the code behind
        await $.ajax({
            type: "POST",
            url: "Edit?handler=UpdateSignature",
            data: JSON.stringify(imageData),
            contentType: "application/json",
            // Needed to allow the app to save files to the wwwroot
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            success: function (msg) {
                console.log("Signature updated: " + msg);
                alert("Signature updated!");
            },
        });
    } catch (err) {
        console.log("Cannot update signature: " + err);
        alert("Cannot update signature!")
    }
}