$(() => {
    $(".deposit").on('click', function () {
        const contid = $(this).data('contid');
        $("[name='contributorID']").val(contid);

        const tr = $(this).closest('tr');
        const name = tr.find('td').eq(1).text();
        $("#deposit-name").text(name);

        new bootstrap.Modal($('#addDeposit')[0]).show();
    });
    $("table").on('click', '#cont-edit', function () {
        const button = $(this);
        const id = button.data('contid');
        const firstName = button.data('first-name');
        const lastName = button.data('last-name');
        const cellNum = button.data('cell');
        const alwaysInclude = button.data('always-include');
        const date = button.data('date');

        const formatYmd = date => date.toISOString().slice(0, 10);
        let [month, day, year] = date.split('/')
        const dateObj = new Date(+year, +month - 1, +day)

        const div = $("#addContributor");
        const form = div.closest('form');

        form.find("#edit-id").remove();

        const hidden = `<input type="hidden" id="edit-id" name="id" value="${id}" />`;

        form.append(hidden);

        $("#initial-deposit").hide();

        $("[name='firstName']").val(firstName);
        $("[name='lastName']").val(lastName);
        $("[name='cellNum']").val(cellNum);
        $("[name='alwaysInclude']").prop('checked', alwaysInclude === "True");
        $("[name='dateCreated']").val(formatYmd(dateObj));

        form.find(".modal-title").text("Edit Contributor");

        $("#add-or-update").text("Update");

        new bootstrap.Modal($('#addContributor')[0]).show();

        form.attr('action', '/contributors/edit');
    });
    $("#add-contributor-button").on('click', function () {
        const div = $("#addContributor");
        const form = div.closest('form');

        $("[name='firstName']").val('');
        $("[name='lastName']").val('');
        $("[name='cellNum']").val('');
        $("[name='alwaysInclude']").prop('checked', false);
        $("[name='dateCreated']").val('');

        $("#initial-deposit").show();

        form.find(".modal-title").text("New Contributor");

        form.attr('action', '/contributors/addcontributor');

        $("#add-or-update").text("Add");
    });
    $("#search").on('keyup', function () {
        const text = $(this).val();

        const rows = $("#contributor-rows").find('tr');

        rows.each(function () {
            const row = $(this);
            const name = row.find('td').eq(1).text().toLowerCase();
            if (name.indexOf(text.toLowerCase()) !== -1) {
                row.show()
            } else {
                row.hide()
            }
        });
    });
    $("#clear").on('click', function () {
        $("#search").val('');
        const rows = $("#contributor-rows").find('tr')
        rows.each(function () {
            const row = $(this);
            row.show();
        });
    });
    $("#add-contr").on('submit', function () {
        let index = 0;

        const form = $("#add-contr");

        const rows = $("#contributor-rows").find('tr');

        rows.each(function () {
            const row = $(this);

            const td = row.find('td').eq(4);
            console.log(td);

            td.find("#amt").attr('name', `conts[${index}].amount`);
            td.find("#contid").attr('name', `conts[${index}].contributorid`);
            td.find("#simchaid").attr('name', `conts[${index}].simchaid`);
            index++;
        });
        const checks = $(".form-check");
        checks.find("#contid-include").remove();
        checks.each(function () {
            const check = $(this);
            const checked = check.find("#is-contributing");
            if (checked.prop('checked') === true) {
                const id = check.data('cont-id');
                check.append(`<input id="contid-include" type="hidden" value="${id}" name="contIDs" />`)
            }
        });
    });
});