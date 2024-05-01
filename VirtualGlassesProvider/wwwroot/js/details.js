const errorMessageElement = document.getElementById("errorMessage");
const detailsImage = document.getElementById('detailsImage');
const originalProductImage = detailsImage.src;
const originalProductAltText = detailsImage.alt;
let profileMat;
let productMat;
let faceCascade;

document.getElementById("generateImageBtn").addEventListener("click", function () {
    let selectionVal = document.getElementById("buyFor").value.toString();
    
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "/Home/GetPortrait/?entity=" + selectionVal, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var response = xhr.responseText;
                if (response === "Please login to use this feature" || response === "Please upload a portrait") {
                    reRenderPartialView("", "", response);
                } else {
                    var img = new Image();
                    img.src = response;
                    img.onload = function () {
                        profileMat = cv.imread(img);
                        if (productMat === undefined) {
                            img.src = originalProductImage;
                            img.onload = function () {
                                productMat = cv.imread(img);
                            };
                        }
                        img.remove();
                        if (faceCascade === undefined) {
                            renderCascade();
                        } else {
                            renderAR();
                        }
                    };
                }
            } else {
                reRenderPartialView("", "", "Error Retrieving Data, Please Try Again");
            }
        }
    };
    xhr.send();


    function renderCascade() {
        faceCascade = new cv.CascadeClassifier();
        const pathToCascade = '\\detection\\haarcascade_frontalface_default.xml';
        const utils = new Utils('errorMessage');
        utils.createFileFromUrl(pathToCascade, pathToCascade, () => {
            loadCascade();
        });
        function loadCascade() {
            faceCascade.load(pathToCascade);
            renderAR();
        }
    }

    function renderAR() {
        let gray = new cv.Mat();
        cv.cvtColor(profileMat, gray, cv.COLOR_BGR2GRAY, 0);
        let faces = new cv.RectVector();
        let msize = new cv.Size(0, 0);
        faceCascade.detectMultiScale(gray, faces, 1.09, 7, 0, msize, msize);

        function putGlassesOnFace(productMat, fc, x, y, w, h) {
            const faceWidth = w;
            const faceHeight = h;

            const glassesWidth = faceWidth + 1;
            const glassesHeight = Math.floor(0.50 * faceHeight) + 1;
            const glassesResized = new cv.Mat();
            cv.resize(productMat, glassesResized, new cv.Size(glassesWidth, glassesHeight));

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
            profileMat = putGlassesOnFace(productMat, profileMat, rect.x, rect.y, rect.width, rect.height);
        }
        const imgCanvas = document.getElementById('canvasOutput');
        cv.imshow("canvasOutput", profileMat);
        const arImgSrc = imgCanvas.toDataURL("image/jpeg");
        gray.delete();
        faces.delete();
        profileMat.delete();
        reRenderPartialView(arImgSrc, "Render", "");
    }
});


document.getElementById("downloadImageLink").addEventListener("click", function () {
    const link = document.createElement('a');
    link.download = 'ARGeneratedImage.jpg';
    link.href = document.getElementById('canvasOutput').toDataURL()
    link.click();
});


function reRenderPartialView(img, alt, error) {
    detailsImage.src = img;
    detailsImage.alt = alt;
    errorMessageElement.textContent = error; 
}


document.getElementById("clearImage").addEventListener("click", function () {
    detailsImage.src = originalProductImage;
    detailsImage.alt = originalProductAltText;
    errorMessageElement.textContent = "";
});