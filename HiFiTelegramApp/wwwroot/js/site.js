loadAjax = (url) =>{
    $.ajax({
        url: url,
        type: 'GET',
        success: function(data){
            $('#container').html(data);
        }
    })
}