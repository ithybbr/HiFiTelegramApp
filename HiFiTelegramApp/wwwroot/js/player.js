var audio;
var playerId;
var volume = 0.25;
var duration = 0;
var artistLabel;
var songLabel;
var timeLabel;
var durLabel;
var progress;
var interval;
var nextSong;
var previousSong;
var songId;
var playerloaded = false;
function audioPlayer({ id, songid, artist, name, path }) {
    if (!playerloaded) {
        loadPlayer();
        playerloaded = true;
    }
    songid = songid;
    timeLabel.innerHTML = "0:00";
    progress.value = 0;
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
            playNext();
        },
        onload: () => {
            duration = audio.duration();
            durLabel.innerHTML = convertSeconds(duration);
            progress.step = (1 / duration * 100); // Set step based on duration
        },
        onplay: () => {
            updateTime();
            document.querySelector('.player').style.visibility = "visible";
        }
    });
    previousSong.value = id - 1;
    nextSong.value = id + 1;
    playerId = audio.play();
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
function loadPlayer() {
    artistLabel = document.querySelector('.player #artist');
    songLabel = document.querySelector('.player #song');
    timeLabel = document.querySelector('.player #time');
    durLabel = document.querySelector('.player #duration');
    nextSong = document.querySelector('.player #next');
    previousSong = document.querySelector('.player #previous');
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
        volume = this.value / 100;
        audio.volume(volume);
        console.log('Volume changed to:', this.value);
    });
    progress = document.querySelector('.player #progress');
    progress.addEventListener('input', function () {
        pr = this.value / 100;
        audio.seek(pr * duration);
        console.log('Seek to:', this.value);
    });
    nextSong.addEventListener('click', function () {
        playNext();
    });
    previousSong.addEventListener('click', function () {
        playPrevious();
    });
}
function playPrevious() {
    $.ajax({
        url: "/Download/songby",
        data: { id: previousSong.value },
        type: 'GET',
        success: function (data) {
            console.log(data);
            audioPlayer(data);
        },
        failure: function (xhr, status, error) {
            console.error('Error appending data:', status, error);
        }
    })
}
function playNext() {
    $.ajax({
        url: "/Download/songby",
        data: { id: nextSong.value },
        type: 'GET',
        success: function (data) {
            console.log(data);
            audioPlayer(data);
        },
        failure: function (xhr, status, error) {
            console.error('Error appending data:', status, error);
        }
    })
}