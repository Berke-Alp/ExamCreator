$(function () {

    function clearInputs() {
        for (var i = 1; i < 5; i++) {
            $("frmQ" + i).val("");
            $("frmAnsQ" + i + "A").val("");
            $("frmAnsQ" + i + "B").val("");
            $("frmAnsQ" + i + "C").val("");
            $("frmAnsQ" + i + "D").val("");
        }
    }

    $("#frmSubmit").click(function (e) {
        e.preventDefault();

        let q1 = $("#frmQ1").val().trim();
        let q2 = $("#frmQ2").val().trim();
        let q3 = $("#frmQ3").val().trim();
        let q4 = $("#frmQ4").val().trim();

        let a1a = $("#frmAnsQ1A").val().trim();
        let a1b = $("#frmAnsQ1B").val().trim();
        let a1c = $("#frmAnsQ1C").val().trim();
        let a1d = $("#frmAnsQ1D").val().trim();

        let a2a = $("#frmAnsQ2A").val().trim();
        let a2b = $("#frmAnsQ2B").val().trim();
        let a2c = $("#frmAnsQ2C").val().trim();
        let a2d = $("#frmAnsQ2D").val().trim();

        let a3a = $("#frmAnsQ3A").val().trim();
        let a3b = $("#frmAnsQ3B").val().trim();
        let a3c = $("#frmAnsQ3C").val().trim();
        let a3d = $("#frmAnsQ3D").val().trim();

        let a4a = $("#frmAnsQ4A").val().trim();
        let a4b = $("#frmAnsQ4B").val().trim();
        let a4c = $("#frmAnsQ4C").val().trim();
        let a4d = $("#frmAnsQ4D").val().trim();

        let right1 = $("#frmRightAnswer1").val();
        let right2 = $("#frmRightAnswer2").val();
        let right3 = $("#frmRightAnswer3").val();
        let right4 = $("#frmRightAnswer4").val();

        let qlength = q1.length < 1 || q2.length < 1 || q3.length < 1 || q4.length < 1;
        let a1length = a1a.length < 1 || a1b.length < 1 || a1c.length < 1 || a1d.length < 1;
        let a2length = a2a.length < 1 || a2b.length < 1 || a2c.length < 1 || a2d.length < 1;
        let a3length = a3a.length < 1 || a3b.length < 1 || a3c.length < 1 || a3d.length < 1;
        let a4length = a4a.length < 1 || a4b.length < 1 || a4c.length < 1 || a4d.length < 1;
        let rightans = right1 < 1 || right2 < 1 || right3 < 1 || right4 < 1;

        if (qlength || a1length || a2length || a3length || a4length) {
            toastr["error"]("Tüm soru ve cevap şıklarını doldurmalısınız.", "Hata")
            return
        }

        if (rightans) {
            toastr["error"]("Sorular için doğru şıkkı seçmelisiniz.", "Hata")
            return
        }


        $.ajax({
            url: "/Account/CreateExam",
            method: "post",
            dataType: "json",
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({
                postId: parseInt($("#frmPosts").val()),
                questions: [
                    {
                        query: q1,
                        rightAnswer: parseInt(right1),
                        a: a1a,
                        b: a1b,
                        c: a1c,
                        d: a1d
                    },
                    {
                        query: q2,
                        rightAnswer: parseInt(right2),
                        a: a2a,
                        b: a2b,
                        c: a3c,
                        d: a4d
                    },
                    {
                        query: q3,
                        rightAnswer: parseInt(right3),
                        a: a3a,
                        b: a3b,
                        c: a3c,
                        d: a3d
                    },
                    {
                        query: q4,
                        rightAnswer: parseInt(right4),
                        a: a4a,
                        b: a4b,
                        c: a4c,
                        d: a4d
                    }
                ],
            }),
            success: function (e) {

                if (e.status != "ok") {
                    toastr["error"](e.message, "Hata");
                }
                else {
                    toastr["success"]("Sınav başarıyla oluşturuldu.", "Başarılı");
                    $("#frmSubmit").prop("disabled", true);

                    setTimeout(function () {
                        window.location.href = "/Account";
                    }, 1500)
                }

            }
        })
    });

    // Yazı seçildiğinde
    $("#frmPosts").change(function (e) {

        let afterLoadArea = $("#afterLoadArea");
        let postContent = $("#frmContent");

        let postId = $("#frmPosts").val();

        if (!postId || postId < 1) {
            return;
        }

        afterLoadArea.toggleClass("hidden", true);
        clearInputs();

        $.ajax({
            url: "/Api/Post/Details/" + postId,
            method: "get",
            accepts: "application/json",
            success: function (e) {

                if (e.status != "ok") {
                    toastr["error"](e.message, "Hata");
                    setTimeout(function () {
                        window.location.reload();
                    }, 1000)
                }
                else {
                    afterLoadArea.toggleClass("hidden", false);
                    postContent.html(e.content);
                }

            }
        })

    });
});