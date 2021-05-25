// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var volume = 0.2;
var wavesurfer = WaveSurfer.create({
    container: '#waveForm',
    cursorColor: 'black',
    cursorWidth: 2,
    barHeight: 0.5,
    barWidth: 3,
    waveColor: '#C3073F',
    progressColor: '#950740'
});

$(document).ready(function () {
    var error = $("#error").text();
    if (error.length != 0) {
        setTimeout(function () {
            alert(error);
        }, 1000);
    }
});

$("#mute").on('click', function () {
    var icon = $(this).find("i");
    if (icon.hasClass('fa-volume-up')) {
        icon.removeClass('fa-volume-up');
        icon.addClass('fa-volume-mute');
        wavesurfer.setVolume(0);
        $("#volume").val(0);
    }
    else {
        icon.addClass('fa-volume-up');
        icon.removeClass('fa-volume-mute');
        if (volume == 0) {
            volume = 0.2;
        }
        wavesurfer.setVolume(volume);
        $("#volume").val(volume);
    }
});

$("#playpause").on('click', function () {
    wavesurfer.playPause();
    var icon = $(this).find("i");
    if (icon.hasClass('fa-pause')) {
        icon.removeClass('fa-pause');
        icon.addClass('fa-play');
    }
    else {
        icon.addClass('fa-pause');
        icon.removeClass('fa-play');
    }
});

$("#backward").on('click', function () {
    var songPath = $('.playing').prev().find("input").val();
    var tr = $('.playing').prev();

    if (songPath != undefined) {
        var songName = tr.children().next().html()
        $('.playing').removeClass('playing');
        $("#player").find('p').html(songName);
        tr.addClass('playing');
        wavesurfer.load('/Songs/' + songPath);
    }
    else {
        wavesurfer.seekTo(0);
    }
    
});

wavesurfer.on('finish', function () {
    var songPath = $('.playing').next().find("input").val();
    var tr = $('.playing').next();

    if (songPath != undefined) {
        var songName = tr.children().next().html()
        $('.playing').removeClass('playing');
        $("#player").find('p').html(songName);
        tr.addClass('playing');
        wavesurfer.load('/Songs/' + songPath);
    }

    var icon = $("#playpause").find("i");
    icon.addClass('fa-play');
    icon.removeClass('fa-pause');
});

$("#forward").on('click', function () {
    var songPath = $('.playing').next().find("input").val();
    var tr = $('.playing').next();

    if (songPath != undefined) {
        var songName = tr.children().next().html()
        $('.playing').removeClass('playing');
        $("#player").find('p').html(songName);
        tr.addClass('playing');
        wavesurfer.load('/Songs/' + songPath);
    }
});

wavesurfer.on('ready', function () {
    var icon = $("#playpause").find("i");
    icon.addClass('fa-pause');
    icon.removeClass('fa-play');
    wavesurfer.play();
});

$('.modal').on('shown.bs.modal', function () {
    $(this).find('input:first').focus();
});

$('.play-from-list').on('click', function () {
    wavesurfer.setVolume(volume);
    wavesurfer.load('/Songs/' + $(this).find('input').val());
    var songName = $(this).parent().next().html();
    $("#player").removeClass('d-none').addClass('d-flex');
    $("#player").find('p').html(songName);
    $('tr').removeClass('playing');
    $(this).parent().parent().addClass('playing');
});

$('.fa-trash-alt').on('click', function () {
    var id = $(this).parent().parent().find('input').attr("name");
    $("#SongId").val(id);
    $("#PlaylistId").val(id);
})

$("#volume").on('change', function () {
    volume = $(this).val();
    wavesurfer.setVolume(volume);
    var icon = $('#mute').find("i");
    if ($(this).val() == 0) {
        icon.removeClass('fa-volume-up');
        icon.addClass('fa-volume-mute');
    }
    else {
        icon.addClass('fa-volume-up');
        icon.removeClass('fa-volume-mute');
    }
});

function searchSongOrPlaylist(id) {
    // Declare variables
    var input, filter, table, tr, td, i, txtValue;
    input = $('#' + id);
    console.log(input);
    filter = input.val().toUpperCase();
    table = input.parent().next();
    tr = table.find("tr");

    tr.each(function (i) {
        if (i != 0) {
            td = $(this).children().next();
        }
        if (td) {
            txtValue = td.html();
            if (txtValue.toUpperCase().indexOf(filter) > -1) {
                this.style.display = "";
            }
            else {
                this.style.display = "none";
            }
        }
    });
}

$('#SongFile').on('change', function () {
    var file = $(this)[0].files[0];
    if (file) {
        $('#SongFileLabel').text(file['name']);
    }
});