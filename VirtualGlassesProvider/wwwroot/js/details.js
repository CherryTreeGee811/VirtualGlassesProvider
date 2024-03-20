$(document).ready(function () {
    const displayImageElement = document.getElementById("detailsImage");
    let glassesImageValElement = document.getElementById("glassesImageVal");

    $("#clearImage").click(function () {
        const selectionVal = document.getElementById("buyFor").value.toString();
        const errorMessageElement = document.getElementById("errorMessage");
        $.ajax({
            type: "GET",
            url: "/Home/GetPortrait/",
            data: { 'entity': selectionVal },
            success: function (response) {
                if (response == "Please login to use this feature" || response == "Please upload a portrait") {
                    reRenderPartialView("", "", response)
                }
                else {
                    renderAR(response);
                }
            },
            error: function () {
                reRenderPartialView("", "", "Error Retrieving Data, Please Try Again")
            },
            cache: false
        });

        function renderAR(portrait) {
            let faceCascade = new cv.CascadeClassifier();
            let pathToCascade = '\\detection\\haarcascade_frontalface_default.xml';
            let utils = new Utils('errorMessage');
            utils.createFileFromUrl(pathToCascade, pathToCascade, () => {
            });
            faceCascade.load(pathToCascade);
            let originalPortraitElement = document.getElementById('originalPortrait');
            originalPortraitElement.src = portrait;
            const img = cv.imread(originalPortraitElement);
            const gray = new cv.Mat();
            cv.cvtColor(img, gray, cv.COLOR_BGR2GRAY, 0);
            const faces = new cv.RectVector();
            let msize = new cv.Size(0, 0);
            faceCascade.detectMultiScale(gray, faces, 1.09, 7, 0, msize, msize);
            let productImageElement = document.getElementById('productImage');
            const glasses = cv.imread(productImageElement);
            let frame = img;

            function putGlassesOnFace(glasses, fc, x, y, w, h) {
                const faceWidth = w;
                const faceHeight = h;

                const glassesWidth = faceWidth + 1;
                const glassesHeight = Math.floor(0.50 * faceHeight) + 1;
                const glassesResized = new cv.Mat();
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
            const canvas = document.getElementById('canvasOutput');
            cv.imshow("canvasOutput", frame);
            const arImgSrc = canvas.toDataURL("image/jpeg");
            let render = document.getElementById("localRender");
            render.src = arImgSrc;
            faceCascade.delete();
            gray.delete();
            faces.delete();
            frame.delete();
            glasses.delete();
            //reRenderPartialView(render.src, "Render", "");
            reRenderPartialView(arImgSrc, "Render", "");
        }
    });


    $("#downloadImageLink").click(function () {
        var link = document.createElement('a');
        link.download = 'ARGeneratedImage.jpg';
        link.href = document.getElementById('canvasOutput').toDataURL()
        link.click();
    });

    function reRenderPartialView(img, alt, error) {
        $.ajax({
            type: "GET",
            url: "/Home/RenderDefault/",
            data: { 'glasses': img, 'brandName': alt, 'error' : error },
            success: function (viewHTML) {
                $("#imgRender").html(viewHTML);
            },
            error: function () {
                reRenderPartialView("", "", "Error Displaying AR, Please Try Again")
            }
        });
    }
});

window.addEventListener('load', function () {
    let glassesImageValElement = document.getElementById("glassesImageVal");
    let glassesBrandNameValElement = document.getElementById("glassesBrandNameVal");
    $.ajax({
        type: "GET",
        url: "/Home/RenderDefault/",
        data: { 'glasses': glassesImageValElement.textContent, 'brandName': glassesBrandNameValElement.textContent },
        success: function (viewHTML) {
            $("#imgRender").html(viewHTML);
        },
        error: function (errorData) { onError(errorData); }
    });
})