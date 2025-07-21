document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.nav-link.text-dark[data-url]')
        .forEach(link => {
            link.addEventListener('click', function () {
                console.log('Link clicked:', this.dataset.url);
                loadAjax(this.dataset.url);
            });
        });
});
function loadAjax(url) {
    console.log('Loading URL:', url);
    $.ajax({
        url: url,
        type: 'GET',
        success: function(data){
            $('#append-ajax').html(data);
        }
    })
}
var audio;
var id;
var volume = 0.5;
var artistLabel;
var songLabel;
var timeLabel;
// this thing will not work. have to fix it later.
function audioPlayer(song) {
    artistLabel.text(song.Artist);
    songLabel.text(song.Name);
    audio = new Howl({
        src: [song.Path],
        html5: true,
        volume: volume
    });
    Howler.stop();
    id = audio.play();
    updateTime();
}
function updateTime() {
    while(audio.playing()) {
        var time = audio.seek();
        timeLabel.text(time);
    }
}
document.addEventListener('DOMContentLoaded', () => {
    artistLabel = document.querySelector('.player #artist');
    songLabel = document.querySelector('.player #song');
    timeLabel = document.querySelector('.player #time');
    document.querySelector('.player button')
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
                updateTime();
            }
        });
    var vol = document.querySelector('.player .form-range');
    vol.addEventListener('change', function () {
        volume = this.value;
        audio.volume(volume);
        console.log('Volume changed to:', this.value);
    });
});