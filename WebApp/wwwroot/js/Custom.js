setTimeout(function () {
    $('.alert').alert('close');
}, 2000);

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

function showCountdown(timeLeft) {
    var interval = setInterval(countdown, 1000);

    function countdown() {
        if (--timeLeft > 0) {
            update();
        }
        else {
            clearInterval(interval);
            update();
            completed();
        }
    }

    function update() {
        hours = Math.floor(timeLeft / 3600);
        minutes = Math.floor((timeLeft % 3600) / 60);
        seconds = timeLeft % 60;

        document.getElementById('countdown').innerHTML = '' + hours + ':' + minutes + ':' + seconds;
    }

    function completed() {
        document.write("Auction ended");
    }
}