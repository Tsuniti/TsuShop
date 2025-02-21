$(document).ready(function(){
    var actions = $("table td:last-child").html();
    // Append table with add row form on add new button click
    $(".add-new").click(function(){
        $(this).attr("disabled", "disabled");
        var index = $("table tbody tr:last-child").index();
        var row = '<tr>' +
            '<td><input type="text" class="form-control" name="name" id="name"></td>' +
            '<td><input type="text" class="form-control" name="department" id="department"></td>' +
            '<td><input type="text" class="form-control" name="phone" id="phone"></td>' +
            '<td>' + actions + '</td>' +
            '</tr>';
        $("table").append(row);
        $("table tbody tr").eq(index + 1).find(".add, .edit").toggle();
    });
    // Add row on add button click
    $(document).on("click", ".add", function(){
        var empty = false;
        var input = $(this).parents("tr").find('input[type="text"], input[type="file"]');

        // input.each(function(){
        //     if (!$(this).val()) {
        //         $(this).addClass("error");
        //         empty = true;
        //     } else {
        //         $(this).removeClass("error");
        //     }
        // });
        //
        // $(this).parents("tr").find(".error").first().focus();

        if (!empty) {
            // Обрабатываем файлы (если есть)
            $(this).parents("tr").find('input[type="file"]').each(function(){
                var fileInput = $(this);
                var file = fileInput[0].files[0]; // Получаем первый выбранный файл

                if (file) {
                    var reader = new FileReader();
                    reader.onload = function(e) {
                        // Показываем изображение вместо поля
                        fileInput.parent("td").html('<img src="' + e.target.result + '" alt="Photo" style="width: 50px; height: 50px;">');
                    };
                    reader.readAsDataURL(file);
                } else {
                    fileInput.parent("td").html(''); // Если файл не выбран, оставляем пусто
                }
            });

            // Обновляем текстовые значения
            $(this).parents("tr").find('input[type="text"]').each(function(){
                $(this).parent("td").html($(this).val());
            });

            $(this).parents("tr").find(".add, .edit").toggle();
            $(".add-new").removeAttr("disabled");
        }
    });
    // Edit row on edit button click
    $(document).on("click", ".edit", function(){
        var row = $(this).parents("tr");

        row.find("td:not(:first-child):not(:nth-last-child(-n+4))").each(function(index){
            if (index === 0) { // Вторая ячейка (по индексу 0 после исключённой первой)
                $(this).html('<input type="file" class="form-control file-upload">');
            } else {
                var text = $(this).text().trim();
                $(this).html('<input type="text" class="form-control" value="' + text + '">');
            }
        });

        row.find(".add, .edit").toggle();
        $(".add-new").attr("disabled", "disabled");
    });
    // Delete row on delete button click
    $(document).on("click", ".delete", function(){
        $(this).parents("tr").remove();
        $(".add-new").removeAttr("disabled");
    });
});