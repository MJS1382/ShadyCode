const menuToggle = document.getElementById("menu-toggle");
const menu = document.getElementById("menu");

if (menuToggle && menu) {
  menuToggle.addEventListener("click", () => {
    const isOpen = menu.classList.toggle("open");
    menuToggle.classList.toggle("open", isOpen);
    console.log("menu is " + (isOpen ? "open" : "closed"));
  });
}

document.getElementById("menu-toggle").addEventListener("click", () => console.log("FF"));

window.hideBootstrapModal = (selector) => {
    const element = document.querySelector(selector);

    if (!element) return;

    const modal = bootstrap.Modal.getOrCreateInstance(element);
    modal.hide();
};