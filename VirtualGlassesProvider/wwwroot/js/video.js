/*document.addEventListener("DOMContentLoaded", function () {
$("#ApplyGlassesFilterBtn").click(function () {

        });
    });*/
const video = document.getElementById("video");
let model;
let canvas = document.getElementById("canvas");
let ctx = canvas.getContext("2d")

function startWebcam() {
  navigator.mediaDevices
    .getUserMedia({
      video: true,
      audio: false,
    })
    .then((stream) => {
      video.srcObject = stream;
    })
    .catch((error) => {
      console.error(error);
    });
}

const detectFaces = async () => {
    const prediction = await model.estimateFaces(video, false)
    console.log(prediction);
    ctx.drawImage(video, 0, 0, 640, 480);
    prediction.forEach(pred => {
        ctx.beginPath();
        ctx.lineWidth = "4";
        
        ctx.stroke();
        ctx.fillStyle = "red";
        pred.landmarks.forEach((landmark) => {
            ctx.fillRect(landmark[0], landmark[1], 5, 5);
});
    });
};

startWebcam();
video.addEventListener("loadeddata", async () => {
    model = await blazeface.load();
    // Correct usage: Pass a function reference instead of invoking the function
    setInterval(detectFaces, 40);
});