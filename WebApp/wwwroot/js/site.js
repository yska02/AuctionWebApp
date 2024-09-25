// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

setTimeout(function () {
    $('.alert').alert('close');
}, 3500);

function setMinEndTime() {
    var now = new Date();
    var offset = now.getTimezoneOffset() * 60000; // Convert to milliseconds
    var localISOTime = new Date(now.getTime() - offset).toISOString().slice(0, 16); // Local datetime string

    document.getElementById("endTime").setAttribute('min', localISOTime);
}

function validateImageFileType(input) {
    var allowedTypes = ['image/jpeg', 'image/png', 'image/jpg'];
    if (!allowedTypes.includes(input.files[0].type)) {
        alert('Please select a valid image file (jpg, png, or jpeg).');
        input.value = ''; // Clear the file input
        return false;
    }
    return true;
}

function showCountdown(endTime) {
    var interval = setInterval(countdown, 1000);

    function countdown() {
        var now = new Date().getTime();
        var timeLeft = Math.floor((endTime.getTime() - now) / 1000);

        if (--timeLeft > 0) {
            update(timeLeft);
        }
        else {
            clearInterval(interval);
            completed();
        }
    }

    function update(timeLeft) {
        hours = Math.floor(timeLeft / 3600);
        minutes = Math.floor((timeLeft % 3600) / 60);
        seconds = timeLeft % 60;

        hours = hours < 10 ? '0' + hours : hours;
        minutes = minutes < 10 ? '0' + minutes : minutes;
        seconds = seconds < 10 ? '0' + seconds : seconds;

        document.getElementById('countdown').innerHTML = '' + hours + ':' + minutes + ':' + seconds;
    }

    function completed() {
        document.getElementById('countdown').innerHTML = "Auction ended";
    }
}