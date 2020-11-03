function SignPdf() {
    $("form#uploadFilesForm").unbind('submit').bind('submit',
        function () {
            if (document.getElementById("pdfFile").files.length === 0) {
                swal("Necesitamos un PDF", "Selecciona un archivo PDF", "warning");
                return false;
            }

            if (document.getElementById("certFile").files.length === 0) {
                swal("Se requiere un Certificado", "Selecciona un certificado", "warning");
                return false;
            }

            if ($("#certPassword").val() === "") {
                swal("Olvidaste algo...", "Escribe la contraseña del certificado", "warning");
                return false;
            }

            if (document.getElementById("signImage").files.length === 0) {
                swal("La imagén de la firma es necesaria", "Selecciona una imagen de firma", "warning");
                return false;
            }

            if ($("#signVisible").prop("checked")) {
                if ($("#signPosition option:selected").val() === "") {
                    swal("Dondé ponemos la firma?", "Selecciona una ubicación para la firma.", "warning");
                    return false;
                }
            }

            var formdata = new FormData($('form#uploadFilesForm').get(0));
            $.ajax({
                url: this.action,
                type: this.method,
                cache: false,
                processData: false,
                contentType: false,
                data: formdata,
                success: function (data) {
                    if (data.ErrorCode !== "0")
                        swal("Algo salio mal :(", data.ErrorDesc, "error");
                    else {
                        window.location = downloadUrl + '?fileGuid=' +
                            data.FileGuid +
                            '&filename=' +
                            data.FileName;
                        swal("Todo salió bien :)", "Tu archivo firmado esta listo", "success");
                    }
                },
                error: function (data) {
                    swal("Algo salio mal :(", data, "error");
                },
                complete: function () {
                    return;
                }
            });
            return false;
        }).submit();
}

$(document).on('change', '.up', function () {
    var names = [];
    var length = $(this).get(0).files.length;
    for (var i = 0; i < $(this).get(0).files.length; ++i) {
        names.push($(this).get(0).files[i].name);
    }
    // $("input[name=file]").val(names);
    if (length > 2) {
        var fileName = names.join(', ');
        $(this).closest('.form-group').find('.form-control').attr("value", length + " files selected");
    }
    else {
        $(this).closest('.form-group').find('.form-control').attr("value", names);
    }
});

$(function () {
    $(".custom-file-input").on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
    });

    $('#signVisible').change(function () {
        if ($(this).prop('checked')) {
            $("#signPosition").show("slow");
        }
        else {
            $("#signPosition").hide("slow");
        }
    })
})