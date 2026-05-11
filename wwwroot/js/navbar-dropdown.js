const dropdown_button = document.querySelector("#navbar-dropdown-button");
const dropdown_menu = document.querySelector("#navbar-dropdown");

const handle_navbar_dropdown = (e) => {
    if (e.target === dropdown_button) {
        if (dropdown_menu.classList.contains("active")) {
            dropdown_menu.classList.remove("active");
            return;
        }
        dropdown_menu.classList.add("active");
        return;
    }
    if (e.srcElement.offsetParent != dropdown_menu) {
        dropdown_menu.classList.remove("active");
    }
}

window.addEventListener("click", (e) => handle_navbar_dropdown(e));