const video = document.getElementById("video");
const button = document.getElementById("ApplyGlassesFilterBtn"); 
let model;
let canvas = document.getElementById("canvas");
let ctx = canvas.getContext("2d");
let rendering = false;
let stream = null;
let detectFacesIntervalId = null;

function startWebcam() {
    navigator.mediaDevices
        .getUserMedia({
            video: true,
            audio: false,
        })
        .then((s) => {
            stream = s; // Assign the obtained stream to the global variable
            video.srcObject = stream;
        })
        .catch((error) => {
            console.error(error);
        });
}

function stopWebcam() {
    if (stream) {
        stream.getTracks().forEach(track => track.stop());
        stream = null;
    }
    if (video.srcObject) {
        video.srcObject = null;
    }

    if (detectFacesIntervalId !== null) {
        clearInterval(detectFacesIntervalId); // Clear the interval
        detectFacesIntervalId = null;
    }
}

const detectFaces = async () => {
    const prediction = await model.estimateFaces(video, false);
    const glassesImg = new Image();
    glassesImg.src = originalProductImage;
    glassesImg.onload = function () {
        ctx.drawImage(video, 0, 0, 640, 480);
        prediction.forEach(pred => {
            const nose = pred.landmarks[2]; // For vertical positioning adjustment
            const leftEar = pred.landmarks[4]; // Using ears for width calculation
            const rightEar = pred.landmarks[5];

            // Calculate glasses width based on ear positions for a wider fit
            const glassesWidth = rightEar[0] - leftEar[0];
            // Maintain aspect ratio of the glasses image
            const glassesHeight = glassesWidth * (glassesImg.naturalHeight / glassesImg.naturalWidth);

            // Adjust X position to start from the left ear
            const glassesX = leftEar[0];
            // Adjust Y position to be slightly above the nose, taking into account the glasses height
            // This is an arbitrary adjustment to position the glasses more naturally
            const glassesY = nose[1] - (glassesHeight * 1); // You may need to tweak this multiplier

            // Draw the glasses image on the canvas
            ctx.drawImage(glassesImg, glassesX, glassesY, glassesWidth, glassesHeight);
        });
    }
};
$("#ApplyGlassesFilterBtn").click(function () {
    if (!rendering) {
        rendering = true;
        document.getElementById("canvas").style.display = "";
        button.value = "STOP TRY ON!"; // Use textContent or text() for consistency
        startWebcam();
        video.addEventListener("loadeddata", async () => {
            if (!model) { // Load the model only if it hasn't been loaded yet
                model = await blazeface.load();
            }
            detectFacesIntervalId = setInterval(detectFaces, 40); // Start detecting faces
        }, { once: true }); // Optional: Use { once: true } to automatically unregister after firing
    }
    else {
        rendering = false;
        button.value = "LIVE TRY ON!";
        stopWebcam(); // This will now also clear the interval
        document.getElementById("canvas").style.display = "none";
    }
});
