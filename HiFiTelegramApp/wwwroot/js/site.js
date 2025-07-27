document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.nav-link.text-dark[data-url]')
        .forEach(link => {
            link.addEventListener('click', function () {
                console.log('Link clicked:', this.dataset.url);
                loadAjax(this.dataset.url);
            });
        });
});
function links() {
    document.querySelectorAll('.link[data-url]')
        .forEach(link => {
            link.addEventListener('click', function () {
                console.log('Link clicked:', this.dataset.url);
                loadAjax(this.dataset.url);
            });
        });
}
function formAjax() {
    document.querySelectorAll('.form-link[data-url]').
        forEach(link => {
            link.addEventListener('click', function () {
                $.ajax({
                    url: this.dataset.url,
                    type: 'POST',
                    success: function (data) {
                        $('#append-ajax').html(data);
                        links();
                        formAjax(); // Rebind form links after loading new content
                    },
                    error: function (xhr, status, error) {
                        console.error('Error loading form:', status, error);
                    }
                })
            });
        })
}
function loadAjax(url) {
    console.log('Loading URL:', url);
    $.ajax({
        url: url,
        type: 'GET',
        success: function(data){
            $('#append-ajax').html(data);
            links();
        }
    })
}
var audio;
var id;
var volume = 0.25;
var duration = 0;
var artistLabel;
var songLabel;
var timeLabel;
var durLabel;
var progress;
let interval;
function audioPlayer({ songid, artist, name, path }) {
    artistLabel.innerHTML = artist;
    songLabel.innerHTML = name;
    Howler.stop();
    audio = new Howl({
        src: [path],
        html5: true,
        volume: volume,
        onplay: updateTime,   // start polling when play begins
        onend: () => {        // clean up when finished
            timeLabel.innerHTML = "0:00";
            progress.value = 0;
            clearInterval(interval);
        },
        onload: () => {
            duration = audio.duration();
            durLabel.innerHTML = convertSeconds(duration);
            progress.step = (1 / duration * 100); // Set step based on duration
        },
        onplay: () => {
            updateTime();
            document.querySelector('.player').style.visibility = visible;
        }
    });
    id = audio.play();
}
function updateTime() {
    clearInterval(interval);
    let tick = 0;
    interval = setInterval(() => {
        if (!audio.playing()) return clearInterval(interval);
        const time = audio.seek();
        progress.value = (time / duration) * 100;
        timeLabel.innerHTML = convertSeconds(time);
    }, 1000);
}
function convertSeconds(seconds) {
    const minutes = Math.floor(seconds / 60);
    seconds = Math.floor(seconds % 60);
    if (seconds < 10) {
        seconds = '0' + seconds;
    }
    return minutes + ':' + seconds;
}
document.addEventListener('DOMContentLoaded', () => {
    artistLabel = document.querySelector('.player #artist');
    songLabel = document.querySelector('.player #song');
    timeLabel = document.querySelector('.player #time');
    durLabel = document.querySelector('.player #duration');
    document.querySelector('.player #player-ps-btn')
        .addEventListener('click', function () {
            if (audio.playing()) {
                audio.pause();
                this.classList.add('play');
                this.classList.remove('pause');
            }
            else {
                this.classList.remove('play');
                this.classList.add('pause');
                audio.play();
            }
        });
    var vol = document.querySelector('.player #volume');
    vol.addEventListener('input', function () {
        volume = this.value/100;
        audio.volume(volume);
        console.log('Volume changed to:', this.value);
    });
    progress = document.querySelector('.player #progress');
    progress.addEventListener('input', function () {
        pr = this.value / 100;
        audio.seek(pr * duration);
        console.log('Seek to:', this.value);
    });
});