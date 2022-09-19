var cameraStatus = false;
var pictureTaken = false;

async function cameraOn() {
    try {
        // Stream video from the webcam
        let stream = await navigator.mediaDevices.getUserMedia({
            audio: false,
            video: { width: 300, height: 300, facingMode: 'user', },
        });
        webcamVideo.srcObject = stream;
        cameraStatus = true;
        console.log("Camera on.");
        // alert("Camera on.");
    } catch (err) {
        cameraStatus = false;
        console.log("Cannot turn camera on: " + err);
        alert("Cannot turn camera on!")
    }
}

async function takePhoto() {
    try {
        if (cameraStatus) {
            // Capture the video input and display the photo in the photoCanvas
            photoCanvas.getContext("2d").drawImage(webcamVideo, 0, 0, photoCanvas.width, photoCanvas.height);
            pictureTaken = true;
            console.log("Picture taken.");
            alert("Picture taken.")
        }
        else {
            pictureTaken = false;
            console.log("Camera is not on!");
            alert("Camera is not on!")
        }
    } catch (err) {
        pictureTaken = false;
        console.log("Cannot take photo: " + err);
        alert("Cannot take photo!")
        cameraOff();
    }
}

async function savePhoto() {
    try {
        if (cameraStatus && pictureTaken) {
            // Get the raw image data from the data URL, but remove the metadata
            let imageData = photoCanvas.toDataURL("image/png").replace("data:image/png;base64,", "");
            // Send the data, as a string, to the SavePhoto() handler in the code behind
            await $.ajax({
                type: "POST",
                url: "Create?handler=SavePhoto",
                data: JSON.stringify(imageData),
                contentType: "application/json",
                // Needed to allow the app to save files to the wwwroot
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (msg) {
                    console.log("Photo saved: " + msg);
                    alert("Photo saved.");
                }
            });
            cameraOff();
            pictureTaken = false;
        }
        else {
            console.log("No picture taken!");
            alert("No picture taken!")
        }
    } catch (err) {
        console.log("Cannot save photo: " + err);
        alert("Cannot save photo!")
    }
}

async function updatePhoto() {
    try {
        if (cameraStatus && pictureTaken) {
            // Get the raw image data from the data URL, but remove the metadata
            let imageData = photoCanvas.toDataURL("image/png").replace("data:image/png;base64,", "");
            // Send the data, as a string, to the SavePhoto() handler in the code behind
            await $.ajax({
                type: "POST",
                url: "Edit?handler=UpdatePhoto",
                data: JSON.stringify(imageData),
                contentType: "application/json",
                // Needed to allow the app to save files to the wwwroot
                beforeSend: function (xhr) {
                    xhr.setRequestHeader("XSRF-TOKEN",
                        $('input:hidden[name="__RequestVerificationToken"]').val());
                },
                success: function (msg) {
                    console.log("Photo updated: " + msg);
                    alert("Photo updated.");
                }
            });
            cameraOff();
            pictureTaken = false;
        }
        else {
            console.log("No picture taken!");
            alert("No picture taken!")
        }
    } catch (err) {
        console.log("Cannot update photo!");
        alert("Cannot update photo!")
    }
}

async function cameraOff() {
    try {
        if (cameraStatus) {
            // Turn off camera
            webcamVideo.pause();
            webcamVideo.src = "";
            webcamVideo.srcObject.getTracks()[0].stop();
        }
        cameraStatus = false;
        console.log("Camera off.");
        // alert("Camera off.");
    } catch (err) {
        console.log("Cannot turn camera off!");
        alert("Cannot turn camera off!")
    }
}