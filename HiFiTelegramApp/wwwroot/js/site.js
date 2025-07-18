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
function audioPlayer(src, i) {
    audio = new Howl({
        src: [src],
        html5: true,
        volume: 0.5
    });
    Howler.stop();
    id = audio.play();
}
document.addEventListener('DOMContentLoaded', () => {
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
            }
        });
    var vol = document.querySelector('.player .form-range');
    vol.addEventListener('change', function () {
        audio.volume(this.value/100);
        console.log('Volume changed to:', this.value);
    });
});