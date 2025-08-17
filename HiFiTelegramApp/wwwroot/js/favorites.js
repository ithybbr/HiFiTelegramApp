document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('#favorites')) {
        console
        $.ajax({
            url: '/favorite/favorites',
            method: 'GET',
            success: function (data) {
                $('.list-group').html(data);
            },
            error: function (error) {
                console.error('Error fetching favorites:', error);
            }
        })
    }
});