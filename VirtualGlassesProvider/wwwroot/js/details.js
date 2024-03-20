let productImageElement = document.getElementById('productImage');
let errorMessageElement = document.getElementById("errorMessage");
let detailsImage = document.getElementById('detailsImage');
let originalPortraitElement = document.getElementById('originalPortrait');
let hasPathSet = false;

$("#generateImageBtn").click(function () {
    let selectionVal = document.getElementById("buyFor").value.toString();
    $.ajax({
        type: "GET",
        url: "/Home/GetPortrait/",
        data: { 'entity': selectionVal },
        success: function (response) {
            if (response == "Please login to use this feature" || response == "Please upload a portrait") {
                reRenderPartialView("", "", response);
            }
            else {
                originalPortraitElement.src = response;
                renderCascade();
            }
        },
        error: function () {
            reRenderPartialView("", "", "Error Retrieving Data, Please Try Again")
        },
        cache: false
    });

    function renderCascade() {
        let faceCascade = new cv.CascadeClassifier();
        let pathToCascade = '\\detection\\haarcascade_frontalface_default.xml';
        if (!hasPathSet) {
            let utils = new Utils('errorMessage');
            utils.createFileFromUrl(pathToCascade, pathToCascade, () => {
                loadCascade();
                hasPathSet = true
            });
        }
        else {
            loadCascade();
        }
        
        function loadCascade() {
            faceCascade.load(pathToCascade);
            renderAR(faceCascade);
        }
    }

    function renderAR(faceCascade) {
        let img = cv.imread(originalPortraitElement);
        let gray = new cv.Mat();
        cv.cvtColor(img, gray, cv.COLOR_BGR2GRAY, 0);
        let faces = new cv.RectVector();
        let msize = new cv.Size(0, 0);
        faceCascade.detectMultiScale(gray, faces, 1.09, 7, 0, msize, msize);
        let glasses = cv.imread(productImageElement);
        let frame = img;

        function putGlassesOnFace(glasses, fc, x, y, w, h) {
            let faceWidth = w;
            let faceHeight = h;

            let glassesWidth = faceWidth + 1;
            let glassesHeight = Math.floor(0.50 * faceHeight) + 1;
            let glassesResized = new cv.Mat();
            cv.resize(glasses, glassesResized, new cv.Size(glassesWidth, glassesHeight));

            for (let i = 0; i < glassesHeight; ++i) {
                for (let j = 0; j < glassesWidth; ++j) {
                    if (glassesResized.ucharPtr(i, j)[3] !== 0) {
                        for (let k = 0; k < 3; ++k) {
                            fc.ucharPtr(y + i - Math.floor(-0.20 * faceHeight), x + j)[k] = glassesResized.ucharPtr(i, j)[k];
                        }
                    }
                }
            }
            return fc;
        }

        for (let i = 0; i < faces.size(); ++i) {
            const rect = faces.get(i);
            frame = putGlassesOnFace(glasses, frame, rect.x, rect.y, rect.width, rect.height);
        }
        let canvas = document.getElementById('canvasOutput');
        cv.imshow("canvasOutput", frame);
        let arImgSrc = canvas.toDataURL("image/jpeg");
        faceCascade.delete();
        gray.delete();
        faces.delete();
        frame.delete();
        glasses.delete();
        reRenderPartialView(arImgSrc, "Render", "");
    }
});


document.getElementById("downloadImageLink").addEventListener("click", function () {
    var link = document.createElement('a');
    link.download = 'ARGeneratedImage.jpg';
    link.href = document.getElementById('canvasOutput').toDataURL()
    link.click();
});


function reRenderPartialView(img, alt, error) {
    detailsImage.src = img;
    detailsImage.alt = alt;
    errorMessageElement.textContent = error; 
}


document.getElementById("clearImage").addEventListener("click", function() {
    const glassesBrandNameValElement = document.getElementById("glassesBrandNameVal");
    detailsImage.src = productImageElement.src;
    detailsImage.alt = glassesBrandNameValElement.textContent;
    errorMessageElement.textContent = "";
});