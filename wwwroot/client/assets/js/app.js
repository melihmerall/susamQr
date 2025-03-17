"use strict";

// Framework7 nesnesini burada tanımlayacağız
var app;

document.addEventListener("DOMContentLoaded", function () {
    // Framework7'yi başlat
    app = new Framework7({
        root: '#app',
        theme: 'md',
        routes: routes || [],
        view: {
            pushState: true
        },
        externalLinks: '.external',
        routes: routes || [],
        on: {
            pageInit: function () {
                document.querySelectorAll("a[target='_blank']").forEach(link => {
                    link.setAttribute("external", "true");
                });
            }
        }
    });

    var mainView = app.views.create('.view-main', {
        url: '/Home/Index'
    });

    document.querySelectorAll("a.external").forEach(link => {
        link.addEventListener("click", function (e) {
            e.stopPropagation();
        });
    });

    var searchInput = document.getElementById("searchInput");
    if (searchInput) {
        searchInput.addEventListener("keyup", function () {
            var query = this.value.trim();

            if (query.length < 3) {
                document.getElementById("searchResults").innerHTML = "";

                return;
            }

            app.request({
                url: "/Search",
                method: "GET",
                data: { q: query },
                dataType: "json",
                success: function (data) {
                    var html = "";
                    if (data.length === 0) {
                        html = "<p class='no-result'>Sonuç bulunamadı.</p>";
                    } else {
                        data.forEach(function (product) {
                            html += `
                              <div class="search-item">
        <a href="/urun/${product.slug}/">
            <div class="search-img-container">
                <img src="${product.imageUrl}" alt="${product.name}" class="search-img">
                <div>                <h4 class="search-title">${product.name}</h4>
                <p class="search-price">${product.price.toFixed(2)}₺</p></div>

            </div>
        </a>
    </div>
                            `;
                        });

                    }
                    document.getElementById("searchResults").innerHTML = html;
                },
                error: function (xhr, status) {
                    console.error("Arama sırasında hata oluştu:", status);
                    document.getElementById("searchResults").innerHTML = "<p class='error'>Arama sırasında hata oluştu.</p>";
                }
            });
        });
    }



    var modal = document.getElementById("intro-modal");
    var video = document.getElementById("intro-video");
    var skipBtn = document.getElementById("skip-intro");

    // Daha önce video izlendi mi?
    if (localStorage.getItem("introSeen") === "true") {
        modal.style.display = "none"; // Eğer izlendi ise gösterme
    } else {
        modal.style.display = "flex"; // İlk defa açılıyorsa göster
    }

    // Kullanıcı "Menüyü Gör" butonuna bastığında
    skipBtn.addEventListener("click", function () {
        localStorage.setItem("introSeen", "true"); // Kullanıcı gördü, bir daha gösterme
        modal.style.display = "none";
    });

    // Video bittiğinde de bir daha gösterme
    video.addEventListener("ended", function () {
        localStorage.setItem("introSeen", "true"); // Video bitti, bir daha gösterme
        modal.style.display = "none";
    });

    var insta = "";
    var phone = "";

    fetch("/api/settings")
        .then(response => response.json()) // JSON olarak işle
        .then(data => {
            insta = data.instagram;
            phone = data.whatsApp;
        })
        .catch(error => console.error("API'den veri alınamadı:", error));

    var instagramLink = document.getElementById("instagram-link");

    if (instagramLink) {
        instagramLink.addEventListener("click", function (event) {
            event.preventDefault(); // Varsayılan tıklamayı engelle


            insta = decodeURIComponent(insta).trim(); // HTML entity çözüp boşlukları temizle

            if (insta && insta.startsWith("http")) {
                window.open(insta, "_blank"); // Yeni sekmede aç
            }
        });
    }
    var whatsappLink = document.getElementById("whatsapp-link");

    if (whatsappLink) {
        whatsappLink.addEventListener("click", function (event) {
            event.preventDefault(); // Varsayılan tıklama olayını engelle

            var phoneNumber = decodeURIComponent(phone) // HTML entity'yi çöz
                .replace(/\D/g, '') // Sadece rakamları bırak
                .replace(/^0+/, ''); // Başındaki gereksiz sıfırları kaldır

            if (phoneNumber) {
                var waLink = "https://wa.me/" + phoneNumber;
                window.open(waLink, "_blank"); // Yeni sekmede aç
            } else {
                console.log(phone + phoneNumber);
            }
        });
    }
});
